using ExchangeService.APIApp.Models;
using System.Text.Json;

namespace ExchangeService.APIApp
{
    public interface IExchangeService
    {
        Task<double> GetExchangeRateAsync(string inputCurrency, string outputCurrency);
    }

    public class ExchangeServiceHelper : IExchangeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _targetUrl;
        private readonly ILogger<ExchangeServiceHelper> _logger;
        private readonly IHostEnvironment _env;

        public ExchangeServiceHelper(IConfiguration config
            , ILogger<ExchangeServiceHelper> logger
            , IHostEnvironment environment
            , IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _env = environment;
            _targetUrl = config["ExchangeRate:ApiUrl"]
                ?? throw new Exception("Exchange rate url is missing.");
        }

        public async Task<double> GetExchangeRateAsync(string inputCurrency, string outputCurrency)
        {
            if (_env.IsDevelopment())
            {
                _logger.LogInformation($"Input currecy: {inputCurrency}, output currency: {outputCurrency}");
            }
            
            try
            {
                var response = await _httpClient.GetAsync(_targetUrl);

                var json = await response.Content.ReadAsStringAsync();

                if (_env.IsDevelopment())
                {
                    _logger.LogInformation($"Got response content: {json}");
                }

                var data = JsonSerializer.Deserialize<ExchangeRateObject>(json);

                return "success".Equals(data?.Result) ? (data.Rates?.USD ?? 0)
                    : throw new Exception("Failed to get the exchange rate.");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
