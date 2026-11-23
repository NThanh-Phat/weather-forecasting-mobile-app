using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Forecasts
{
    // Thông tin chi tiết của một lần dự báo
    public class ForecastRunDetailResponse
    {
        public int ForecastRunId { get; set; }

        public int ProvinceId { get; set; }

        public string ProvinceName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Thông tin thời tiết tại thời điểm dự báo
        public CurrentWeatherDto CurrentWeather { get; set; } = new();

        // Danh sách 24 mốc nhiệt độ dự báo
        public List<ForecastDetailDto> Forecasts { get; set; } = [];
    }
}