namespace SmartMeal.Application.Common;

public sealed record Error
{
    public required string Code { get; init; }
    public required string Message { get; init; }

    public static Error Create(string code, string message) => new()
    {
        Code = code,
        Message = message
    };

    public static Error FromException(Exception exception) => new()
    {
        Code = exception.GetType().Name,
        Message = exception.Message
    };
}
