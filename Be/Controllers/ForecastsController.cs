
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Be.DTOs.Forecasts;
using Be.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Be.Controllers
{
    [ApiController]
    [Route("api/forecasts")]
    public class ForecastsController : ControllerBase
    {
        private readonly IForecastService _forecastService;

        public ForecastsController(IForecastService forecastService)
        {
            _forecastService = forecastService;
        }

        // POST: api/forecasts
        // Tạo dự báo thời tiết mới cho một tỉnh/thành phố
        [HttpPost]
        public async Task<IActionResult> CreateForecast(
            [FromBody] CreateForecastRequest request)
        {
            try
            {
                var result = await _forecastService.CreateForecastAsync(request);

                return Ok(new
                {
                    success = true,
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

        // GET: api/forecasts/latest/{provinceId}
        // Lấy trạng thái và dự báo mới nhất của nhiều tỉnh
        [HttpGet("latest/{provinceId:int}")]
        public async Task<IActionResult> GetLatestForecast(int provinceId)
        {
            try
            {
                var result = await _forecastService.GetLatestForecastAsync(provinceId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy dữ liệu dự báo cho tỉnh/thành phố này."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy dữ liệu dự báo mới nhất thành công.",
                    data = result
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
                    message = "Đã xảy ra lỗi khi lấy dữ liệu dự báo.",
                    error = ex.Message
                });
            }
        }

        // GET: api/forecasts/latest?provinceIds=1&provinceIds=2
        // Lấy dự báo mới nhất của nhiều tỉnh/thành phố
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestForecasts(
            [FromQuery] List<int> provinceIds)
        {
            try
            {
                // Kiểm tra danh sách tỉnh
                if (provinceIds == null || provinceIds.Count == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Vui lòng cung cấp danh sách ProvinceId."
                    });
                }

                // Lấy dữ liệu dự báo từ Service
                var results = await _forecastService.GetLatestForecastsAsync(
                    provinceIds
                );

                // Trả kết quả
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách dự báo mới nhất thành công.",
                    total = results.Count,
                    data = results
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
                    message = "Đã xảy ra lỗi khi lấy danh sách dự báo.",
                    error = ex.Message
                });
            }
        }
    }
}
