using System.Text.Json.Serialization;

namespace ExchangeService.APIApp.Models
{
    public class ExchangeRateObject
    {
        [JsonPropertyName("result")]
        public string Result { get; set; } = string.Empty;
        [JsonPropertyName("base_code")]
        public string BaseCode { get; set; } = string.Empty;
        [JsonPropertyName("rates")]
        public Dictionary<string, double> Rates { get; set; } = new Dictionary<string, double>();
    }
}