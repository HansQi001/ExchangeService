using ExchangeService.APIApp.Helpers;
using ExchangeService.APIApp.Models;
using System.Text.Json;

namespace ExchangeService.APIApp
{
    public interface IExchangeService
    {
        Task<double> GetExchangeRateAsync(string inputCurrency, string outputCurrency, CancellationToken cancellationToken);
    }

    public class ExchangeService : IExchangeService
    {
        private readonly IHttpHelper _httpHelper;
        private readonly string _targetUrl;
        private readonly ILogger<ExchangeService> _logger;
        private readonly IHostEnvironment _env;

        public ExchangeService(IConfiguration config
            , ILogger<ExchangeService> logger
            , IHostEnvironment environment
            , IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
            _logger = logger;
            _env = environment;
            _targetUrl = config["ExchangeRate:ApiUrl"]
                ?? throw new Exception("Exchange rate url is missing.");
        }

        public async Task<double> GetExchangeRateAsync(string inputCurrency, string outputCurrency, CancellationToken cancellationToken)
        {
            if (_env.IsDevelopment())
            {
                _logger.LogInformation($"Input currecy: {inputCurrency}, output currency: {outputCurrency}");
            }

            try
            {
                var url = string.Format(_targetUrl, inputCurrency);
                var data = await _httpHelper.GetAsync<ExchangeRateObject>(url, cancellationToken);

                if (_env.IsDevelopment())
                {
                    _logger.LogInformation($"Got response content: {JsonSerializer.Serialize(data)}");
                }

                return "success".Equals(data?.Result) ? (data.Rates?[outputCurrency] ?? 0)
                    : throw new Exception("Failed to get the exchange rate.");
            }

            catch (Exception)
            {

                throw;
            }
        }
    }
}
