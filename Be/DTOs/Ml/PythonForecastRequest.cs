using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Ml
{
    public class PythonForecastRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}