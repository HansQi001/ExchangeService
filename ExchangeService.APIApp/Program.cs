using ExchangeService.APIApp.Models;

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
            builder.Services.AddHttpClient(); ;

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IExchangeService, ExchangeServiceHelper>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapPost("/ExchangeService", async (ExchangeDTO dto, IExchangeService exchangeServiceHelper) =>
            {
                dto.ExchangeRate = await exchangeServiceHelper.GetExchangeRateAsync(dto.InputCurrency, dto.OutputCurrency);

                return dto;
            })
            .WithName("PostExchangeService")
            .WithOpenApi();

            app.Run();
        }
    }
}
