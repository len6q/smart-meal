namespace SmartMeal.Domain;

public sealed record MenuItem
{
    public required string Id { get; init; }
    public required string Article { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required bool IsWeighted { get; init; }
    public required string FullPath { get; init; }
    public required IReadOnlyList<string> Barcodes { get; init; }
}
