using System.Text.Json.Serialization;

namespace ExchangeService.APIApp.Models
{
    public class ExchangeRateObject
    {
        [JsonPropertyName("result")]
        public string Result { get; set; }
        [JsonPropertyName("base_code")]
        public string BaseCode { get; set; }
        [JsonPropertyName("rates")]
        public ConversionRate Rates { get; set; }
    }

    public class ConversionRate
    {
        public double AUD { get; set; }
        public double USD { get; set; }
    }
}