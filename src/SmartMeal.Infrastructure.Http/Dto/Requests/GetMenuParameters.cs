namespace SmartMeal.Infrastructure.Http.Dto.Requests;

public sealed record GetMenuParameters
{
    public required bool WithPrice { get; init; }
}
