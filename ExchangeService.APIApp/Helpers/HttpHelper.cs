using System.Text.Json;

namespace ExchangeService.APIApp.Helpers
{
    public interface IHttpHelper
    {
        Task<T?> GetAsync<T>(string targetUrl, CancellationToken cancellationToken);
    }

    public class HttpHelper : IHttpHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpHelper> _logger;

        public HttpHelper(IHttpClientFactory httpClientFactory
            , ILogger<HttpHelper> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ExchangeClient");
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string targetUrl, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(targetUrl, cancellationToken);

                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                var data = JsonSerializer.Deserialize<T>(json);

                return data ?? default;
            }
            catch (JsonException je)
            {
                _logger.LogError("Got error: {0}", je.Message);
                return default;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
