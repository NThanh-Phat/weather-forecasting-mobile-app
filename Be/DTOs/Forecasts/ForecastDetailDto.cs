using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Forecasts
{
    public class ForecastDetailDto
    {
        public DateTime ForecastTime {get; set;}
        public double PredictedTemperature {get; set;} 
    }
}