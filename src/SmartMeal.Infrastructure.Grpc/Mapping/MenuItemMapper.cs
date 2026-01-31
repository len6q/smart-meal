using SmartMeal.Domain;
using GrpcMenuItem = Sms.Test.MenuItem;

namespace SmartMeal.Infrastructure.Grpc.Mapping;

public static class MenuItemMapper
{
    public static MenuItem ToDomain(this GrpcMenuItem grpcItem) => new()
    {
        Id = grpcItem.Id,
        Article = grpcItem.Article,
        Name = grpcItem.Name,
        Price = (decimal)grpcItem.Price,
        IsWeighted = grpcItem.IsWeighted,
        FullPath = grpcItem.FullPath,
        Barcodes = grpcItem.Barcodes.ToList().AsReadOnly()
    };

    public static IReadOnlyList<MenuItem> ToDomain(
        this IEnumerable<GrpcMenuItem> grpcItems) =>
        grpcItems.Select(ToDomain).ToList().AsReadOnly();
}
