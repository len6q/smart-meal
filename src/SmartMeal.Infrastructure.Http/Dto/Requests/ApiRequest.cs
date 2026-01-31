namespace SmartMeal.Infrastructure.Http.Dto.Requests;

public sealed record ApiRequest<T>
{
    public required string Command { get; init; }
    public required T CommandParameters { get; init; }
}
