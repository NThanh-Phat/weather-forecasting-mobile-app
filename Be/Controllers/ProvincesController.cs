using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Be.DTOs.Provinces;
using Be.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvincesController : Controller
    {
        private readonly IProvinceService _provinceService;

        public ProvincesController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }   

        [HttpGet]
        public async Task<ActionResult<List<ProvinceResponse>>> GetAllProvinces()
        {
            var provinces = await _provinceService.GetAllProvincesAsync();
            return Ok(provinces);
        }
    }
}