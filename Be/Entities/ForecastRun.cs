using System.ComponentModel.DataAnnotations.Schema;

namespace Be.Entities
{
    [Table("ForecastRuns")]
    public class ForecastRun
    {
        public int ForecastRunId { get; set; }  
        public int ProvinceId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public double? CurrentTemperature { get; set; }

        public double? CurrentRelativeHumidity { get; set; }

        public double? CurrentSurfacePressure { get; set; }

        public double? CurrentWindSpeed { get; set; }

        public double? CurrentShortwaveRadiation { get; set; }
    }
}