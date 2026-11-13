using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Forecasts
{
    public class CurrentWeatherDto
    {
        public double? Temperature { get; set; }

        public double? RelativeHumidity { get; set; }

        public double? SurfacePressure { get; set; }

        public double? WindSpeed { get; set; }

        public double? ShortwaveRadiation { get; set; }
    }
}