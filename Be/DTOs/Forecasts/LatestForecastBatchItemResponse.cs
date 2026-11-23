using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Forecasts
{
    public class LatestForecastBatchItemResponse
    {
        public int ProvinceId { get; set; }

        public string? ProvinceName { get; set; }

        public bool ProvinceExists { get; set; }

        public bool HasForecast { get; set; }

        public string Message { get; set; } = string.Empty;

        public LatestForecastResponse? Forecast { get; set; }        
    }
}