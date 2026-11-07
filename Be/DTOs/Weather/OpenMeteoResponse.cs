using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Be.DTOs.Weather
{
    public class OpenMeteoResponse
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        
        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [JsonPropertyName("hourly")]
        public OpenMeteoHourly Hourly { get; set; } = new();
    }

    public class OpenMeteoHourly
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = [];

        [JsonPropertyName("temperature_2m")]
        public List<double?> Temperature { get; set; } = [];

        [JsonPropertyName("relative_humidity_2m")]
        public List<double?> RelativeHumidity { get; set; } = [];

        [JsonPropertyName("surface_pressure")]
        public List<double?> SurfacePressure { get; set; } = [];

        [JsonPropertyName("precipitation")]
        public List<double?> Precipitation { get; set; } = [];

        [JsonPropertyName("cloud_cover")]
        public List<double?> CloudCover { get; set; } = [];

        [JsonPropertyName("wind_speed_10m")]
        public List<double?> WindSpeed { get; set; } = [];

        [JsonPropertyName("shortwave_radiation")]
        public List<double?> ShortwaveRadiation { get; set; } = [];
    }
}