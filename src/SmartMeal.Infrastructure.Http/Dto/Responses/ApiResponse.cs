namespace SmartMeal.Infrastructure.Http.Dto.Responses;

public record ApiResponse
{
    public required string Command { get; init; }
    public required bool Success { get; init; }
    public required string ErrorMessage { get; init; }
}

public sealed record ApiResponse<T> : ApiResponse
{
    public required T Data { get; init; }
}
