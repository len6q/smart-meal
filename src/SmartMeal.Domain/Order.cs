namespace SmartMeal.Domain;

public sealed record Order
{
    public required Guid Id { get; init; }
    public required IReadOnlyList<OrderItem> Items { get; init; }
}
