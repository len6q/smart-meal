namespace SmartMeal.Infrastructure.Http;

public sealed record HttpClientOptions
{
    public required string BaseUrl { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
}
