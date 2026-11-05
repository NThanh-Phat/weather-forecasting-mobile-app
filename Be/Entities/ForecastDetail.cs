using System.ComponentModel.DataAnnotations.Schema;

namespace Be.Entities
{
    [Table("ForecastDetails")]
    public class ForecastDetail
    {
        public int ForecastDetailId { get; set; }
        public int ForecastRunId { get; set; }
        public DateTime ForecastTime { get; set; }
        public double PredictedTemperature { get; set; }
    }
}