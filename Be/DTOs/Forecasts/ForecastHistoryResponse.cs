using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Forecasts
{
    // Kết quả lịch sử dự báo có phân trang
    public class ForecastHistoryResponse
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public List<ForecastHistoryDto> Items { get; set; } = [];        
    }
}