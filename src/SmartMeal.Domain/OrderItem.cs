namespace SmartMeal.Domain;

public sealed record OrderItem
{
    public required string MenuItemId { get; init; }
    public required decimal Quantity { get; init; }
}
