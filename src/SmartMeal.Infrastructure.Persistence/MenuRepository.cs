using Microsoft.EntityFrameworkCore;
using SmartMeal.Application.Abstractions;
using SmartMeal.Domain;

namespace SmartMeal.Infrastructure.Persistence;

public sealed class MenuRepository : IMenuRepository
{
    private readonly ApplicationDbContext _context;

    public MenuRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveMenuItemsAsync(
        IEnumerable<MenuItem> items,
        CancellationToken cancellationToken = default)
    {
        var itemsList = items.ToList();
        if (itemsList.Count == 0)
        {
            return;
        }

        var itemIds = itemsList.Select(i => i.Id).ToList();
        var existingIds = await _context.MenuItems
            .Where(x => itemIds.Contains(x.Id))
            .Select(x => x.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var existingIdsSet = existingIds.ToHashSet();

        var itemsToAdd = itemsList.Where(item => !existingIdsSet.Contains(item.Id));
        var itemsToUpdate = itemsList.Where(item => existingIdsSet.Contains(item.Id));

        _context.MenuItems.UpdateRange(itemsToUpdate);
        await _context.MenuItems.AddRangeAsync(itemsToAdd, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<MenuItem?> GetByArticleAsync(
        string article,
        CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Article == article, cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

