using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Be.DTOs.Provinces;

namespace Be.Services.Interfaces
{
    public interface IProvinceService
    {
        Task<List<ProvinceResponse>> GetAllProvincesAsync();
    }
}