using MarketAnalysis.Core.Settings;
using MarketAnalysis.Data.Context;
using MarketAnalysis.Schedular.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

        AlphaVantageSettings? alphaVantageSettings = configuration.GetSection(nameof(AlphaVantageSettings)).Get<AlphaVantageSettings>();
        string? dbConnectionString = configuration.GetConnectionString("DefaultConnection");

        var services = new ServiceCollection();
        Console.WriteLine(Directory.GetCurrentDirectory());
        services.AddDbContext<MarketAnalysisDbContext>(options => options.UseSqlite(dbConnectionString));
        services.AddScoped<IMarketAnalysisRepository, MarketAnalysisRepository>();

        if (alphaVantageSettings != null)
        {
            services.AddSingleton(alphaVantageSettings);
            services.AddTransient<IPriceFetcher, AlphaVantagePriceFetcher>();
        }

        services.AddTransient<SchedularService>();

        var provider = services.BuildServiceProvider();

        if (args?.FirstOrDefault() == "DELTA")
        {
            await (provider.GetRequiredService<SchedularService>()?.ImportDelta() ?? Task.CompletedTask);
        }
        else
        {
            await (provider.GetRequiredService<SchedularService>()?.ImportFull() ?? Task.CompletedTask);
        }
    }
}