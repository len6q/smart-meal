namespace SmartMeal.Infrastructure.Grpc;

public sealed record GrpcClientOptions
{
    public required string Address { get; init; }
}
