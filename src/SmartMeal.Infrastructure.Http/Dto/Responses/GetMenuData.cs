namespace SmartMeal.Infrastructure.Http.Dto.Responses;

public sealed record GetMenuData
{
    public required List<MenuItemDto> MenuItems { get; init; }
}
