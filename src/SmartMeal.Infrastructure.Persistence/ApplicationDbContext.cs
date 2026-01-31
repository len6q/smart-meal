using Microsoft.EntityFrameworkCore;
using SmartMeal.Domain;
using SmartMeal.Infrastructure.Persistence.Configurations;

namespace SmartMeal.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new MenuItemConfiguration());
    }
}
