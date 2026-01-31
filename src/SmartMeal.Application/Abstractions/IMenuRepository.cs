using SmartMeal.Domain;

namespace SmartMeal.Application.Abstractions;

public interface IMenuRepository
{
    Task SaveMenuItemsAsync(IEnumerable<MenuItem> items, CancellationToken cancellationToken = default);
    Task<MenuItem?> GetByArticleAsync(string article, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default);
}
