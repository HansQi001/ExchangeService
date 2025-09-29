using ExchangeService.APIApp.Helpers;
using Polly;
using Polly.Extensions.Http;

namespace ExchangeService.APIApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Register HttpClient
            builder.Services.AddHttpClient("ExchangeClient")
                .AddPolicyHandler(policy => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))  // set retry
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5)); // set timeout

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IHttpHelper, HttpHelper>();
            builder.Services.AddSingleton<IExchangeService, ExchangeService>();

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // disable https redirection to satisfy the curl command in the Assignment, it's http
            // curl -X 'POST' 'http://localhost:5050/ExchangeService' -H 'accept: text/plain' -H 'Content-Type: application/json' -d '{ "amount": 5, "inputCurrency": "AUD", "outputCurrency": "USD"}' 
            //app.UseHttpsRedirection(); 

            app.MapControllers();

            app.Run();
        }
    }
}
