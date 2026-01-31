using Microsoft.Extensions.DependencyInjection;
using SmartMeal.Application.Abstractions;

namespace SmartMeal.Infrastructure.Http;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpMenuApiClient(
        this IServiceCollection services,
        HttpClientOptions options)
    {
        services.AddHttpClient<HttpMenuApiClient>()
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

        services.AddSingleton(options);
        services.AddTransient<IMenuApiClient, HttpMenuApiClient>();

        return services;
    }
}
