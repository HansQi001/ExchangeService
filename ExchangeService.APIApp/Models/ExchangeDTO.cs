using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExchangeService.APIApp.Models
{
    public class ExchangeDTO
    {
        [Required]
        [Range(0, double.MaxValue)]
        public double Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string InputCurrency { get; set; } = "AUD";

        [Required]
        [MaxLength(3)]
        public string OutputCurrency { get; set; } = "USD";

        [JsonIgnore]
        public double ExchangeRate { get; set; } = 0;

        public string Value => (Amount * ExchangeRate).ToString("#.##");
    }
}