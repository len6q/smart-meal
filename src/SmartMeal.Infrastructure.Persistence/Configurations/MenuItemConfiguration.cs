using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartMeal.Domain;

namespace SmartMeal.Infrastructure.Persistence.Configurations;

public sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Article)
            .HasColumnName("article")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.IsWeighted)
            .HasColumnName("is_weighted")
            .IsRequired();

        builder.Property(x => x.FullPath)
            .HasColumnName("full_path")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Barcodes)
            .HasColumnName("barcodes")
            .HasConversion(
                v => v.ToArray(),
                v => v.ToList().AsReadOnly())
            .IsRequired();

        builder.HasIndex(x => x.Article)
            .HasDatabaseName("ix_menu_items_article");
    }
}
