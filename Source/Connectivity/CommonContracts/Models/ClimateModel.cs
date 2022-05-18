using System.Text.Json.Serialization;

namespace CommonContracts.Models
{
    public class ClimateModel
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("temperature")]
        public string Temperature { get; set; }

        [JsonPropertyName("pressure")]
        public string Pressure { get; set; }

        [JsonPropertyName("humdity")]
        public string Humidity { get; set; }
    }
}