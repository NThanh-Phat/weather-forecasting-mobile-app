namespace Be.Entities
{
    public class Province
    {
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}