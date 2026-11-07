using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Be.DTOs.Ml;

namespace Be.Services.Interfaces
{
    public interface IPythonMLService
    {
        Task<PythonForecastResponse> PredictAsync(
            PythonForecastRequest request,
            CancellationToken cancellationToken = default);

    }
}