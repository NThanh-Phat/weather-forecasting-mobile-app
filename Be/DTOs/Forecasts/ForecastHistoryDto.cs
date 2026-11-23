using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Forecasts
{
    // Thông tin một lần dự báo trong lịch sử
    public class ForecastHistoryDto
    {
        public int ForecastRunId { get; set; }

        public string ProvinceName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}