using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Forecasts
{
    public class LatestForecastResponse
    {
        public int ForecastRunId { get; set; }

        public int ProvinceId { get; set; }

        public string ProvinceName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public CurrentWeatherDto CurrentWeather { get; set; } = new();

        public List<ForecastDetailDto> Forecasts { get; set; } = [];
    }
}