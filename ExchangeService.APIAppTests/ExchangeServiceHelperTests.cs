using ExchangeService.APIApp;
using ExchangeService.APIApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Text.Json;


namespace ExchangeService.APIAppTests
{
    public class ExchangeServiceHelperTests
    {
        private readonly Mock<IConfiguration> configMoq;
        private readonly Mock<ILogger<ExchangeServiceHelper>> logMoq;
        private readonly Mock<IHostEnvironment> envMoq;
        private readonly Mock<IHttpClientFactory> mockFactory;

        public ExchangeServiceHelperTests()
        {
            configMoq = new Mock<IConfiguration>();
            configMoq.Setup(c => c["ExchangeRate:ApiUrl"]).Returns("https://open.er-api.com/v6/latest/{0}");

            logMoq = new Mock<ILogger<ExchangeServiceHelper>>();

            envMoq = new Mock<IHostEnvironment>();
            envMoq.Setup(e => e.EnvironmentName).Returns("Development");

            mockFactory = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task Should_Returns_Valid_Rate()
        {
            var input = "AUD";
            var output = "USD";

            var exchangeObject = new ExchangeRateObject
            {
                Result = "success",
                BaseCode = input,
                Rates = new Dictionary<string, double>
                {
                    { "AUD",1 },
                    { "USD",0.67 }
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

            var helper = new ExchangeServiceHelper(configMoq.Object, logMoq.Object, envMoq.Object, mockFactory.Object);

            var rate = await helper.GetExchangeRateAsync(input, output, CancellationToken.None);

            Assert.True(rate > 0);
        }
    }
}