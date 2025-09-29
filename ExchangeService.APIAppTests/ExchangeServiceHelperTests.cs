using ExchangeService.APIApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;


namespace ExchangeService.APIAppTests
{
    public class ExchangeServiceHelperTests
    {
        [Fact]
        public async Task Should_Returns_Valid_Rate()
        {
            var configMoq = new Mock<IConfiguration>();
            configMoq.Setup(c => c["ExchangeRate:ApiUrl"]).Returns("https://open.er-api.com/v6/latest/AUD");

            var logMoq = new Mock<ILogger<ExchangeServiceHelper>>();

            var envMoq = new Mock<IHostEnvironment>();
            envMoq.Setup(e => e.EnvironmentName).Returns("Development");

            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var input = "AUD";
            var output = "USD";

            var helper = new ExchangeServiceHelper(configMoq.Object, logMoq.Object, envMoq.Object, mockFactory.Object);

            var rate = await helper.GetExchangeRateAsync(input, output);

            Assert.True(rate > 0);
        }
    }
}