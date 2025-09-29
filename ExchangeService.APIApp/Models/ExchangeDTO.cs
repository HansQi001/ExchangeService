using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExchangeService.APIApp.Models
{
    public class ExchangeDTO
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be non-negative.")]
        public double Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency code must be 3 uppercase letters.")]
        public string InputCurrency { get; set; } = "AUD";

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency code must be 3 uppercase letters.")]
        public string OutputCurrency { get; set; } = "USD";

        [JsonIgnore]
        public double ExchangeRate { get; set; } = 0;

        public decimal Value => Math.Round((decimal)(Amount * ExchangeRate), 2);
    }
}