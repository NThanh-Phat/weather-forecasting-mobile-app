
using System.Net;
using System.Net.Http.Json;
using Be.DTOs.Ml;
using Be.Services.Interfaces;

namespace Be.Services;

public class PythonMLService : IPythonMLService
{
    private readonly HttpClient _httpClient;

    public PythonMLService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PythonForecastResponse> PredictAsync(
        PythonForecastRequest request,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.PostAsJsonAsync(
                "/api/predict",
                request,
                cancellationToken
            );
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException(
                "Python API phản hồi quá thời gian cho phép."
            );
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(
                "Không thể kết nối đến Python API.",
                ex
            );
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(
                    cancellationToken
                );

                throw new HttpRequestException(
                    $"Python API trả về HTTP {(int)response.StatusCode}: {error}",
                    null,
                    response.StatusCode
                );
            }

            var result =
                await response.Content.ReadFromJsonAsync<PythonForecastResponse>(
                    cancellationToken: cancellationToken
                );

            if (result == null || !result.Success)
            {
                throw new InvalidOperationException(
                    "Python API không trả về kết quả dự báo hợp lệ."
                );
            }

            if (result.Timezone != "Asia/Bangkok")
            {
                throw new InvalidOperationException(
                    "Múi giờ kết quả dự báo không hợp lệ."
                );
            }

            if (result.ForecastHours != 24 ||
                result.Predictions == null ||
                result.Predictions.Count != 24)
            {
                throw new InvalidOperationException(
                    "Kết quả dự báo phải có đúng 24 giá trị nhiệt độ."
                );
            }

            for (int i = 0; i < result.Predictions.Count; i++)
            {
                var prediction = result.Predictions[i];

                if (double.IsNaN(prediction.PredictedTemperature) ||
                    double.IsInfinity(prediction.PredictedTemperature))
                {
                    throw new InvalidOperationException(
                        $"Nhiệt độ dự báo tại vị trí {i + 1} không hợp lệ."
                    );
                }

                if (i > 0)
                {
                    var previous = result.Predictions[i - 1];

                    if (prediction.ForecastTime !=
                        previous.ForecastTime.AddHours(1))
                    {
                        throw new InvalidOperationException(
                            "Thời gian dự báo không liên tục theo từng giờ."
                        );
                    }
                }
            }

            return result;
        }
    }
}
