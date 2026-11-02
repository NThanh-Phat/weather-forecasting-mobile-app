using Be.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DatabaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                bool isConnected = await _context.Database.CanConnectAsync();

                if (isConnected)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Kết nối SQL Server thành công!",
                        database = "TemperatureForecastDB"
                    });
                }

                return StatusCode(503, new
                {
                    success = false,
                    message = "Không thể kết nối đến SQL Server."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi kết nối SQL Server.",
                    error = ex.Message
                });
            }
        }
    }
}
