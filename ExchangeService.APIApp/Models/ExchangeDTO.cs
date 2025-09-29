using System.ComponentModel.DataAnnotations;

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

        public double ExchangeRate { private get; set; } = 1;

        public string Value => (Amount * ExchangeRate).ToString("#.##");
    }
}