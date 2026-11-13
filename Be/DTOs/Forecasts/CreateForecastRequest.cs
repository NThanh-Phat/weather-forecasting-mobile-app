using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Forecasts
{
    public class CreateForecastRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "ProvinceId phải lớn hơn 0.")]
        public int ProvinceId { get; set; }
    }
}