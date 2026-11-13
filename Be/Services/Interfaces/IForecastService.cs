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
    }
}