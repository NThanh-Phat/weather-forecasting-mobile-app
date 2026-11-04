using System.ComponentModel.DataAnnotations.Schema;

namespace Be.Entities
{
    [Table("ForecastRuns")]
    public class ForecastRun
    {
        public int ForecastRunId { get; set; }  
        public int ProvinceId { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}