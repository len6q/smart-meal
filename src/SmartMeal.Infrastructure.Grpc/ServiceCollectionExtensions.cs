using Microsoft.Extensions.DependencyInjection;
using SmartMeal.Application.Abstractions;

namespace SmartMeal.Infrastructure.Grpc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcMenuApiClient(
        this IServiceCollection services,
        Action<GrpcClientOptions> configure)
    {
        var options = new GrpcClientOptions
        {
            Address = string.Empty
        };

        configure(options);

        services.AddSingleton(options);
        services.AddTransient<IMenuApiClient, GrpcMenuApiClient>();

        return services;
    }
}
