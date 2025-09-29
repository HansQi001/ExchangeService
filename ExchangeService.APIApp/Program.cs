using ExchangeService.APIApp.Helpers;
using ExchangeService.APIApp.Models;
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapPost("/ExchangeService", async (ExchangeDTO dto, IExchangeService exchangeServiceHelper, CancellationToken cancellationToken) =>
            {
                dto.ExchangeRate = await exchangeServiceHelper.GetExchangeRateAsync(dto.InputCurrency, dto.OutputCurrency, cancellationToken);

                if (dto.ExchangeRate == 0)
                {
                    return Results.NotFound();
                }

                return Results.Ok(dto);
            })
            .WithName("PostExchangeService")
            .WithOpenApi();

            app.Run();
        }
    }
}
