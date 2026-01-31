using SmartMeal.Domain;
using SmartMeal.Infrastructure.Http.Dto.Responses;

namespace SmartMeal.Infrastructure.Http.Mapping;

public static class MenuItemMapper
{
    public static MenuItem ToDomain(this MenuItemDto dto) => new()
    {
        Id = dto.Id,
        Article = dto.Article,
        Name = dto.Name,
        Price = dto.Price,
        IsWeighted = dto.IsWeighted,
        FullPath = dto.FullPath,
        Barcodes = dto.Barcodes.AsReadOnly()
    };

    public static IReadOnlyList<MenuItem> ToDomain(this IEnumerable<MenuItemDto> dtos) =>
        dtos.Select(ToDomain).ToList().AsReadOnly();
}
