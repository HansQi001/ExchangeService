using ExchangeService.APIApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeService.APIApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExchangeServiceController : ControllerBase
    {
        private readonly IExchangeService _exchangeService;
        public ExchangeServiceController(IExchangeService exchangeService)
        {
            _exchangeService = exchangeService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ExchangeDTO dto, CancellationToken cancellationToken)
        {
            dto.ExchangeRate = await _exchangeService.GetExchangeRateAsync(dto.InputCurrency, dto.OutputCurrency, cancellationToken);

            if (dto.ExchangeRate == 0)
            {
                return NotFound();
            }

            return Ok(dto);
        }
    }
}
