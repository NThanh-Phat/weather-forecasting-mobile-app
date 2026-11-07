using System.Globalization;
using be.DTOs.Weather;
using be.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;

namespace be.Services
{
    public class OpenMeteoService : IOpenMeteoService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OpenMeteoService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<OpenMeteoResponse> GetWeatherAsync(
            double latitude,
            double longitude
        )
        {
            //Kiem tra toa do hop le
            if (latitude < -90 || latitude > 90 ||
                longitude < -180 || longitude > 180)
            {
                throw new ArgumentOutOfRangeException("Tọa độ không hợp lệ.");
            }


            // Tao HttpClient dang ky voi ten "OpenMeteo" trong Program.cs
            var client = _httpClientFactory.CreateClient("OpenMeteo");

            //chuyen toa do sang chuoi
            var latitudeValue = latitude.ToString(CultureInfo.InvariantCulture);
            var longitudeValue = longitude.ToString(CultureInfo.InvariantCulture);
            
            //Xay dung URL goi API
            var requestUrl =
            $"/v1/forecast?latitude={latitudeValue}" +
            $"&longitude={longitudeValue}" +
            "&hourly=temperature_2m,relative_humidity_2m,surface_pressure,precipitation,cloud_cover,wind_speed_10m,shortwave_radiation" +
            "&timezone=Asia%2FBangkok" +
            "&past_days=8" +
            "&forecast_days=1";

            //Gui request den Open-Meteo
            using var response = await client.GetAsync(requestUrl);
            
            // Kiem tra trang thai tra ve
            response.EnsureSuccessStatusCode();

            //chuyen JSON Thanh DTO
            var weatherData = await response.Content.ReadFromJsonAsync<OpenMeteoResponse>();

            if (weatherData == null)
            {
                throw new Exception("Không thể đọc dữ liệu trả về từ Open-Meteo.");
            }

            //Kiem tra du lieu lich su
            if (weatherData.Hourly.Time.Count < 192) // 8 ngay * 24 gio = 192   
            {
                throw new Exception("Không có dữ liệu lịch sử thời tiết.");
            }
            
            
            //Kiem tra so luong du lieu cua cac bien thoi tiet
            var hourly = weatherData.Hourly;

            int expectedCount = hourly.Time.Count;
            
            if (hourly.Temperature.Count != expectedCount ||
                hourly.RelativeHumidity.Count != expectedCount ||
                hourly.SurfacePressure.Count != expectedCount ||
                hourly.Precipitation.Count != expectedCount ||
                hourly.CloudCover.Count != expectedCount ||
                hourly.WindSpeed.Count != expectedCount ||
                hourly.ShortwaveRadiation.Count != expectedCount)
            {
                throw new Exception(
                    "Dữ liệu thời tiết không đồng bộ."
                    );
            }

            //Kiem Tra timeStamp
            var  timeStamps = new List<DateTime>();
            foreach (var time in hourly.Time)
            {
                if (!DateTime.TryParseExact(
                    time,
                    "yyyy-MM-dd'T'HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var timeStamp))   
                {
                    throw new InvalidOperationException(
                        $"Timestamp không hợp lệ: {time}'."
                        );
                }
                timeStamps.Add(timeStamp);
            }

            for (int i = 1; i < timeStamps.Count; i++)
            {
                if(timeStamps[i] - timeStamps[i - 1] != TimeSpan.FromHours(1)) 
                {
                    throw new InvalidOperationException(
                        "Timestamp bị trùng hoặc không liên tục theo giờ."
                        );
                }
            }

            //kiem tra du lieu thieu hoac ko hop le
            if(HasMissingOrInvalidValue(hourly.Temperature) ||
               HasMissingOrInvalidValue(hourly.RelativeHumidity) ||
               HasMissingOrInvalidValue(hourly.SurfacePressure) ||
               HasMissingOrInvalidValue(hourly.Precipitation) ||
               HasMissingOrInvalidValue(hourly.CloudCover) ||
               HasMissingOrInvalidValue(hourly.WindSpeed) ||
               HasMissingOrInvalidValue(hourly.ShortwaveRadiation))
            {
                throw new InvalidOperationException(
                    "Dữ liệu thời tiết có giá trị thiếu hoặc không hợp lệ."
                    );
            }
            
            //Kiem tra pham vi gia tri cua cac bien thoi tiet
            if (hourly.Temperature.Any(t => t < -100 || t > 100) ||
                hourly.RelativeHumidity.Any(h => h < 0 || h > 100) ||
                hourly.SurfacePressure.Any(p => p < 800 || p > 1200) ||
                hourly.Precipitation.Any(p => p < 0 || p > 500) ||
                hourly.CloudCover.Any(c => c < 0 || c > 100) ||
                hourly.WindSpeed.Any(w => w < 0 || w > 150) ||
                hourly.ShortwaveRadiation.Any(s => s < 0 || s > 2000))
            {
                throw new InvalidOperationException(
                    "Dữ liệu thời tiết có giá trị vượt quá phạm vi hợp lệ."
                    );
            }
            return weatherData;
        }

        // Hàm kiểm tra giá trị thiếu hoặc không hợp lệ trong danh sách
        private static bool HasMissingOrInvalidValue(
            List<double?> values)
        {
            return values.Any(
                v => !v.HasValue ||
                     !double.IsFinite(v.Value)
                );  
        }
    }
}