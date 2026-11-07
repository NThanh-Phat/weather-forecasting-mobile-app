
using Be.DTOs.Ml;
using Be.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Be.Controllers;

[ApiController]
[Route("api/python-ml/test")]
public class PythonMLTestController : ControllerBase
{
    private readonly IPythonMLService _pythonMLService;

    public PythonMLTestController(
        IPythonMLService pythonMLService)
    {
        _pythonMLService = pythonMLService;
    }

    [HttpGet]
    public async Task<IActionResult> TestPrediction(
        [FromQuery] double latitude = 10.0452,
        [FromQuery] double longitude = 105.7469,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new PythonForecastRequest
            {
                Latitude = latitude,
                Longitude = longitude
            };

            var result = await _pythonMLService.PredictAsync(
                request,
                cancellationToken
            );

            return Ok(result);
        }
        catch (TimeoutException ex)
        {
            return StatusCode(
                StatusCodes.Status504GatewayTimeout,
                new { message = ex.Message }
            );
        }
        catch (HttpRequestException ex)
        {
            var statusCode = ex.StatusCode;

            // Python trả lỗi HTTP hoặc BE không kết nối được Python
            if (statusCode == HttpStatusCode.NotFound ||
                statusCode == HttpStatusCode.BadRequest)
            {
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new { message = "Python API trả về lỗi.", detail = ex.Message }
                );
            }

            return StatusCode(
                StatusCodes.Status502BadGateway,
                new { message = "Không thể kết nối Python API.", detail = ex.Message }
            );
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new { message = "Kết quả từ Python không hợp lệ.", detail = ex.Message }
            );
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "Lỗi nội bộ Backend.", detail = ex.Message }
            );
        }
    }
}
