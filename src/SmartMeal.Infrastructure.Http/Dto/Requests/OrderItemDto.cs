namespace SmartMeal.Infrastructure.Http.Dto.Requests;

public sealed record OrderItemDto
{
    public required string Id { get; init; }
    public required string Quantity { get; init; }
}
