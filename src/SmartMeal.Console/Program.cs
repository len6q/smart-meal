using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SmartMeal.Console;
using SmartMeal.Infrastructure.Persistence;
using SmartMeal.Infrastructure.Http;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: $"test-sms-console-app-{DateTime.Now:yyyyMMdd}.log",
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day);
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("PostgreSQL connection string not found");

        services.AddPersistence(connectionString);

        var httpOptions = new HttpClientOptions
        {
            BaseUrl = configuration["ApiSettings:BaseUrl"]
                ?? throw new InvalidOperationException("ApiSettings:BaseUrl not found"),
            Username = configuration["ApiSettings:Username"]
                ?? throw new InvalidOperationException("ApiSettings:Username not found"),
            Password = configuration["ApiSettings:Password"]
                ?? throw new InvalidOperationException("ApiSettings:Password not found")
        };

        services.AddHttpMenuApiClient(httpOptions);

        services.AddHostedService<ConsoleAppService>();
    })
    .Build();

await host.RunAsync();
