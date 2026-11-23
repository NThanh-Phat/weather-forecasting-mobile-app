using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Be.Data;
using Be.DTOs.Forecasts;
using Be.DTOs.Ml;
using Be.Entities;
using Be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Be.Services
{
    public class ForecastService : IForecastService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOpenMeteoService _openMeteoService;
        private readonly IPythonMLService _pythonMLService;

        public ForecastService(
            ApplicationDbContext context,
            IOpenMeteoService openMeteoService,
            IPythonMLService pythonMLService)
        {
            _context = context;
            _openMeteoService = openMeteoService;
            _pythonMLService = pythonMLService;
        }
        public async Task<ForecastResponse> CreateForecastAsync(
            CreateForecastRequest request
        )
        {
            var totalWatch = Stopwatch.StartNew();
            var watch = new Stopwatch();
            // 1. Kiem tra ProvinceId
            if (request.ProvinceId <= 0)
            {
                throw new ArgumentException(
                    "ProvinceId phải lớn hơn 0.");
            }

            // 2. Lay thong tin tinh tu database
            var province = await _context.Provinces
                .FirstOrDefaultAsync(
                    p => p.ProvinceId == request.ProvinceId);

            if (province == null)
            {
                throw new KeyNotFoundException(
                    "Không tìm thấy tỉnh/thành phố.");
            }

            // 3. Lay du lieu thoi tiet tu Open-Meteo
            watch.Restart();

            var weatherData = await _openMeteoService.GetWeatherAsync(
                province.Latitude,
                province.Longitude);

            watch.Stop();

            Console.WriteLine(
                $"[PERFORMANCE] Open-Meteo: {watch.ElapsedMilliseconds} ms");

            var hourly = weatherData.Hourly;

            // 4. Xac dinh moc thoi gian hien tai theo gio Viet Nam
            var vietnamNow = DateTime.UtcNow.AddHours(7);

            int currentIndex = -1;

            for (int i = 0; i < hourly.Time.Count; i++)
            {
                var timestamp = DateTime.ParseExact(
                    hourly.Time[i],
                    "yyyy-MM-dd'T'HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture);

                if (timestamp <= vietnamNow)
                {
                    currentIndex = i;
                }
                else
                {
                    break;
                }
            }

            if (currentIndex < 0)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy dữ liệu thời tiết tại thời điểm hiện tại.");
            }

            // 5. Tao CurrentWeather
            var currentWeather = new CurrentWeatherDto
            {
                Temperature = hourly.Temperature[currentIndex],
                RelativeHumidity = hourly.RelativeHumidity[currentIndex],
                SurfacePressure = hourly.SurfacePressure[currentIndex],
                WindSpeed = hourly.WindSpeed[currentIndex],
                ShortwaveRadiation = hourly.ShortwaveRadiation[currentIndex]
            };

            // 6. Goi Python ML Service
            var pythonRequest = new PythonForecastRequest
            {
                Latitude = province.Latitude,
                Longitude = province.Longitude
            };

            watch.Restart();

            var pythonResult = await _pythonMLService.PredictAsync(
                pythonRequest);

            watch.Stop();

            Console.WriteLine(
                $"[PERFORMANCE] Python ML: {watch.ElapsedMilliseconds} ms");

            // 7. Kiem tra ket qua du bao
            if (pythonResult == null || !pythonResult.Success)
            {
                throw new InvalidOperationException(
                    "Python ML không trả về kết quả dự báo hợp lệ.");
            }

            if (pythonResult.Timezone != "Asia/Bangkok")
            {
                throw new InvalidOperationException(
                    "Múi giờ dự báo không hợp lệ.");
            }

            if (pythonResult.ForecastHours != 24 ||
                pythonResult.Predictions == null ||
                pythonResult.Predictions.Count != 24)
            {
                throw new InvalidOperationException(
                    "Kết quả dự báo phải có đúng 24 mốc thời gian.");
            }

            if (pythonResult.Predictions.Any(
                p => !double.IsFinite(p.PredictedTemperature)))
            {
                throw new InvalidOperationException(
                    "Nhiệt độ dự báo có giá trị không hợp lệ.");
            }

            // 8. Tao ForecastRun
            watch.Restart();

            var forecastRun = new ForecastRun
            {
                ProvinceId = province.ProvinceId,
                CreatedAt = vietnamNow,

                // Lưu thời tiết hiện tại
                CurrentTemperature = currentWeather.Temperature,
                CurrentRelativeHumidity = currentWeather.RelativeHumidity,
                CurrentSurfacePressure = currentWeather.SurfacePressure,
                CurrentWindSpeed = currentWeather.WindSpeed,
                CurrentShortwaveRadiation = currentWeather.ShortwaveRadiation
            };

            _context.ForecastRuns.Add(forecastRun);

            await _context.SaveChangesAsync();

            // 9. Tao 24 ForecastDetails
            var forecastDetails = pythonResult.Predictions
                .Select(p => new ForecastDetail
                {
                    ForecastRunId = forecastRun.ForecastRunId,
                    ForecastTime = p.ForecastTime,
                    PredictedTemperature = p.PredictedTemperature
                })
                .ToList();

            _context.ForecastDetails.AddRange(forecastDetails);

            await _context.SaveChangesAsync();

            watch.Stop();

            Console.WriteLine(
                $"[PERFORMANCE] SQL Save: {watch.ElapsedMilliseconds} ms");

            // 10. Tao response
            var response = new ForecastResponse
            {
                ForecastRunId = forecastRun.ForecastRunId,
                ProvinceName = province.ProvinceName,
                CreatedAt = forecastRun.CreatedAt,
                CurrentWeather = currentWeather,
                Forecasts = pythonResult.Predictions
                    .Select(p => new ForecastDetailDto
                    {
                        ForecastTime = p.ForecastTime,
                        PredictedTemperature = p.PredictedTemperature
                    })
                    .ToList()
            };

            // 11. Tong thoi gian xu ly
            totalWatch.Stop();

            Console.WriteLine("--------------------------------");
            Console.WriteLine(
                $"[PERFORMANCE] TOTAL: {totalWatch.ElapsedMilliseconds} ms");
            Console.WriteLine("--------------------------------");

            return response;
        }
        public async Task<LatestForecastResponse?> GetLatestForecastAsync(
            int provinceId)
        {
            if (provinceId <= 0)
                throw new ArgumentException("ProvinceId phải lớn hơn 0.");

            // 1. Kiểm tra tỉnh/thành phố
            var province = await _context.Provinces
                .FirstOrDefaultAsync(p => p.ProvinceId == provinceId);

            if (province == null)
                return null;

            // 2. Lấy lần dự báo mới nhất
            var latestRun = await _context.ForecastRuns
                .Where(r => r.ProvinceId == provinceId)
                .OrderByDescending(r => r.CreatedAt)
                .ThenByDescending(r => r.ForecastRunId)
                .FirstOrDefaultAsync();

            if (latestRun == null)
                return null;

            // 3. Lấy 24 mốc dự báo
            var forecastDetails = await _context.ForecastDetails
                .Where(d => d.ForecastRunId == latestRun.ForecastRunId)
                .OrderBy(d => d.ForecastTime)
                .Select(d => new ForecastDetailDto
                {
                    ForecastTime = d.ForecastTime,
                    PredictedTemperature = d.PredictedTemperature
                })
                .ToListAsync();

            // 4. Trả về kết quả
            return new LatestForecastResponse
            {
                ForecastRunId = latestRun.ForecastRunId,
                ProvinceId = province.ProvinceId,
                ProvinceName = province.ProvinceName,
                CreatedAt = latestRun.CreatedAt,

                CurrentWeather = new CurrentWeatherDto
                {
                    Temperature = latestRun.CurrentTemperature,
                    RelativeHumidity = latestRun.CurrentRelativeHumidity,
                    SurfacePressure = latestRun.CurrentSurfacePressure,
                    WindSpeed = latestRun.CurrentWindSpeed,
                    ShortwaveRadiation = latestRun.CurrentShortwaveRadiation
                },

                Forecasts = forecastDetails
            };
        }

        // Lấy trạng thái và dự báo mới nhất của nhiều tỉnh
        public async Task<List<LatestForecastBatchItemResponse>> GetLatestForecastsAsync(
            List<int> provinceIds)
        {
            // Kiểm tra danh sách đầu vào
            if (provinceIds == null || provinceIds.Count == 0)
                throw new ArgumentException("Danh sách ProvinceId không được để trống.");

            if (provinceIds.Any(id => id <= 0))
                throw new ArgumentException("Tất cả ProvinceId phải lớn hơn 0.");

            // Loại bỏ ProvinceId trùng lặp
            var uniqueProvinceIds = provinceIds.Distinct().ToList();

            var results = new List<LatestForecastBatchItemResponse>();

            foreach (var provinceId in uniqueProvinceIds)
            {
                // Kiểm tra tỉnh có tồn tại
                var province = await _context.Provinces
                    .FirstOrDefaultAsync(p => p.ProvinceId == provinceId);

                if (province == null)
                {
                    results.Add(new LatestForecastBatchItemResponse
                    {
                        ProvinceId = provinceId,
                        ProvinceName = null,
                        ProvinceExists = false,
                        HasForecast = false,
                        Message = "Không tìm thấy tỉnh/thành phố.",
                        Forecast = null
                    });

                    continue;
                }

                // Lấy dự báo mới nhất của tỉnh
                var forecast = await GetLatestForecastAsync(provinceId);

                if (forecast == null)
                {
                    results.Add(new LatestForecastBatchItemResponse
                    {
                        ProvinceId = provinceId,
                        ProvinceName = province.ProvinceName,
                        ProvinceExists = true,
                        HasForecast = false,
                        Message = "Tỉnh/thành phố chưa có dữ liệu dự báo.",
                        Forecast = null
                    });

                    continue;
                }

                // Tỉnh đã có dữ liệu dự báo
                results.Add(new LatestForecastBatchItemResponse
                {
                    ProvinceId = provinceId,
                    ProvinceName = province.ProvinceName,
                    ProvinceExists = true,
                    HasForecast = true,
                    Message = "Đã có dữ liệu dự báo.",
                    Forecast = forecast
                });
            }

            return results;
        }
    }
}