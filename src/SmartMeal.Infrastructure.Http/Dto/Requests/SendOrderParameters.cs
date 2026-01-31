namespace SmartMeal.Infrastructure.Http.Dto.Requests;

public sealed record SendOrderParameters
{
    public required string OrderId { get; init; }
    public required List<OrderItemDto> MenuItems { get; init; }
}
