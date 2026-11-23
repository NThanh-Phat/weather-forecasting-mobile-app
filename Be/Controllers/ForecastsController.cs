
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Be.DTOs.Forecasts;
using Be.Services.Interfaces;
using Be.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Be.Controllers
{
    [ApiController]
    [Route("api/forecasts")]
    public class ForecastsController : ControllerBase
    {
        private readonly IForecastService _forecastService;
        private readonly ILogger<ForecastsController> _logger;

        public ForecastsController(
            IForecastService forecastService,
            ILogger<ForecastsController> logger)
        {
            _forecastService = forecastService;
            _logger = logger;
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
                    message = ex.Message,
                    provinceId = request.ProvinceId,
                    errorCode = "PROVINCE_NOT_FOUND"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    errorCode = "INVALID_REQUEST"
                });
            }
            catch (ForecastInProgressException ex)
            {
                return Conflict(new
                {
                    success = false,
                    message = ex.Message,
                    provinceId = request.ProvinceId,
                    errorCode = "FORECAST_IN_PROGRESS"
                });
            }
            catch (Exception ex)
            {
                // Ghi lỗi chi tiết vào log Backend
                _logger.LogError(
                    ex,
                    "Lỗi khi thực hiện dự báo cho ProvinceId {ProvinceId}",
                    request.ProvinceId
                );

                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.",
                    errorCode = "INTERNAL_SERVER_ERROR"
                });
            }
        }

        // GET: api/forecasts/latest/{provinceId}
        // Lấy dự báo mới nhất của một tỉnh/thành phố
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
                        message = "Tỉnh/thành phố chưa có dữ liệu dự báo.",
                        provinceId = provinceId,
                        errorCode = "FORECAST_NOT_FOUND"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy dữ liệu dự báo mới nhất thành công.",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message,
                    provinceId = provinceId,
                    errorCode = "PROVINCE_NOT_FOUND"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    errorCode = "INVALID_REQUEST"
                });
            }
            catch (Exception ex)
            {
                // Ghi lỗi chi tiết vào log Backend
                _logger.LogError(
                    ex,
                    "Lỗi khi lấy dự báo cho ProvinceId {ProvinceId}",
                    provinceId
                );

                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.",
                    errorCode = "INTERNAL_SERVER_ERROR"
                });
            }
        }

        // GET: api/forecasts/latest?provinceIds=1&provinceIds=2
        // Lấy trạng thái và dự báo mới nhất của nhiều tỉnh/thành phố
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
                        message = "Vui lòng cung cấp danh sách ProvinceId.",
                        errorCode = "INVALID_REQUEST"
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
                    message = ex.Message,
                    errorCode = "INVALID_REQUEST"
                });
            }
            catch (Exception ex)
            {
                // Ghi lỗi chi tiết vào log Backend
                _logger.LogError(
                    ex,
                    "Lỗi khi lấy danh sách dự báo nhiều tỉnh."
                );

                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.",
                    errorCode = "INTERNAL_SERVER_ERROR"
                });
            }
        }
    }
}
