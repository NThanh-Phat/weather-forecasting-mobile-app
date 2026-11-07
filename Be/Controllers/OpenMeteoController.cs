using Be.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Be.Controllers
{
    [ApiController]
    [Route("api/open-meteo")]
    public class OpenMeteoController : Controller
    {
        private readonly IOpenMeteoService _openMeteoService;

        public OpenMeteoController(IOpenMeteoService openMeteoService)
        {
            _openMeteoService = openMeteoService;
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestOpenMeteo(
            [FromQuery] double latitude = 10.0452,
            [FromQuery] double longitude = 105.7469
        )
        {
            try
            {
                var weatherData = await _openMeteoService.GetWeatherAsync(
                    latitude,
                    longitude
                );

                return Ok(weatherData);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new
                {
                    message = "Không thể kết nối đến Open-Meteo.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi khi lấy dữ liệu thời tiết.",
                    error = ex.Message
                });
            }
        }
    }
}