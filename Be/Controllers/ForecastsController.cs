using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Be.DTOs.Forecasts;
using Be.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Be.Controllers
{
    [ApiController]
    [Route("api/forecats")]
    public class ForecastsController : Controller
    {
        private readonly IForecastService _forecastService;

        public ForecastsController(IForecastService forecastService)
        {
            _forecastService = forecastService;
        }

        //Post: api/forecasts
        [HttpPost]
        public async Task<IActionResult> CreateForecast(
            [FromBody] CreateForecastRequest request)
        {
            try
            {
                var result = await _forecastService.CreateForecastAsync (request);

                return Ok(new
                {
                    success =true,
                    message = "Dự báo thời tiết thành công.",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi khi thực hiện dự báo.",
                    error = ex.Message
                });
            }
        }
    }
}

