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

namespace Be.Services
{
    public class ForecastService: IForecastService
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
            var weatherData = await _openMeteoService.GetWeatherAsync(
                province.Latitude,
                province.Longitude);

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

            var pythonResult = await _pythonMLService.PredictAsync(
                pythonRequest);

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
            var forecastRun = new ForecastRun
            {
                ProvinceId = province.ProvinceId,
                CreatedAt = vietnamNow
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

            return response;
        }
    }
}