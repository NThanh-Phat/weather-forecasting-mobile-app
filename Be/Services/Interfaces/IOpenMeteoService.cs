using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Be.DTOs.Weather;

namespace Be.Services.Interfaces
{
    public interface IOpenMeteoService
    {
        Task<OpenMeteoResponse> GetWeatherAsync(
            double latitude,
            double longitude
        );
    }
}