using ExchangeService.APIApp.Helpers;
using ExchangeService.APIApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;


namespace ExchangeService.APIAppTests
{
    public class ExchangeServiceHelperTests
    {
        private readonly Mock<IConfiguration> configMoq;
        private readonly Mock<IHostEnvironment> envMoq;
        private readonly Mock<IHttpClientFactory> mockFactory;

        public ExchangeServiceHelperTests()
        {
            configMoq = new Mock<IConfiguration>();
            configMoq.Setup(c => c["ExchangeRate:ApiUrl"]).Returns("https://open.er-api.com/v6/latest/{0}");

            envMoq = new Mock<IHostEnvironment>();
            envMoq.Setup(e => e.EnvironmentName).Returns("Development");

            mockFactory = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task Should_Returns_Valid_Rate()
        {
            var input = "AUD";
            var output = "USD";
            var fakeRate = 0.67;

            var exchangeObject = new ExchangeRateObject
            {
                Result = "success",
                BaseCode = input,
                Rates = new Dictionary<string, double>
                {
                    { "AUD",1 },
                    { "USD",fakeRate }
                }
            };
            var rateString = JsonSerializer.Serialize(exchangeObject);

            var handlerMoq = new Mock<HttpMessageHandler>();
            handlerMoq
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(rateString)
                });

            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handlerMoq.Object));

            var logMoq1 = new Mock<ILogger<HttpHelper>>();
            var httpHelper = new HttpHelper(mockFactory.Object, logMoq1.Object);

            var logMoq2 = new Mock<ILogger<APIApp.ExchangeService>>();
            var helper = new APIApp.ExchangeService(configMoq.Object, logMoq2.Object, envMoq.Object, httpHelper);

            var rate = await helper.GetExchangeRateAsync(input, output, CancellationToken.None);

            Assert.Equal(fakeRate, rate);
        }
    }
}