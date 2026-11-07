using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Be.DTOs.Ml
{
    public class PythonForecastResponse
    {
        public bool Success { get; set; }
        public string Timezone { get; set; } = string.Empty;
        public int ForecastHours { get; set; }
        public List<PythonPredictionItem> Predictions { get; set; } = [];
    }
    
    public class PythonPredictionItem
    {
        public DateTime ForecastTime { get; set; }
        public double PredictedTemperature { get; set; }
    }
}