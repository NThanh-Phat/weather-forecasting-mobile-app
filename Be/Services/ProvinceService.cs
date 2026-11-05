using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Be.DTOs.Provinces;
using Be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Be.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly Be.Data.ApplicationDbContext _context;

        public ProvinceService(Be.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProvinceResponse>> GetAllProvincesAsync()
        {
            var provinces = await _context.Provinces
                .AsNoTracking()
                .ToListAsync();

            return provinces.Select(p => new ProvinceResponse
            {
                ProvinceId = p.ProvinceId,
                ProvinceName = p.ProvinceName,
                Latitude = p.Latitude,
                Longitude = p.Longitude
            }).ToList();
        }
    }
}