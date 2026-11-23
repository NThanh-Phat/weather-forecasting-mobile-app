using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Be.DTOs.Forecasts;

namespace Be.Services.Interfaces
{
    public interface IForecastService
    {
        Task<ForecastResponse> CreateForecastAsync(
            CreateForecastRequest request
        );

        Task<LatestForecastResponse?> GetLatestForecastAsync(
            int provinceId
        );

        // Lấy trạng thái và dự báo mới nhất của nhiều tỉnh
        Task<List<LatestForecastBatchItemResponse>> GetLatestForecastsAsync(
            List<int> provinceIds
        );
    }
}