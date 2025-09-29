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

        public ExchangeServiceHelper(IConfiguration config
            , IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _targetUrl = config["ExchangeRate:ApiUrl"]
                ?? throw new Exception("Exchange rate url is missing.");
        }

        public async Task<double> GetExchangeRateAsync(string inputCurrency, string outputCurrency)
        {
            try
            {
                var response = await _httpClient.GetAsync(_targetUrl);

                var json = await response.Content.ReadAsStringAsync();
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
