using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SmartMeal.Wpf.Services;
using SmartMeal.Wpf.ViewModels;
using SmartMeal.Wpf.Views;

namespace SmartMeal.Wpf;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                path: $"test-sms-wpf-app-{DateTime.Now:yyyyMMdd}.log",
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("Приложение запущено");

        var services = new ServiceCollection();

        var variableNames = configuration.GetSection("EnvironmentVariables").Get<string[]>()
            ?? Array.Empty<string>();

        services.AddLogging(builder =>
        {
            builder.AddSerilog(dispose: true);
        });

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IEnvironmentVariableService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<EnvironmentVariableService>>();
            return new EnvironmentVariableService(variableNames, logger);
        });
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Приложение завершено");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
