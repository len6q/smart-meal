using FluentAssertions;
using SmartMeal.Infrastructure.Http.Dto.Responses;
using SmartMeal.Infrastructure.Http.Mapping;

namespace SmartMeal.Tests;

public sealed class MenuItemMapperTests
{
    [Fact]
    public void ToDomain_SingleItem_MapsAllFieldsCorrectly()
    {
        var dto = new MenuItemDto
        {
            Id = "item-123",
            Article = "ART001",
            Name = "Test Product",
            Price = 99.99m,
            IsWeighted = true,
            FullPath = "/category/subcategory/product",
            Barcodes = new List<string> { "1234567890123", "9876543210987" }
        };

        var domain = dto.ToDomain();

        domain.Id.Should().Be("item-123");
        domain.Article.Should().Be("ART001");
        domain.Name.Should().Be("Test Product");
        domain.Price.Should().Be(99.99m);
        domain.IsWeighted.Should().BeTrue();
        domain.FullPath.Should().Be("/category/subcategory/product");
        domain.Barcodes.Should().HaveCount(2);
        domain.Barcodes[0].Should().Be("1234567890123");
        domain.Barcodes[1].Should().Be("9876543210987");
    }

    [Fact]
    public void ToDomain_SingleItem_BarcodesAreReadOnly()
    {
        var dto = new MenuItemDto
        {
            Id = "item-123",
            Article = "ART001",
            Name = "Test Product",
            Price = 99.99m,
            IsWeighted = false,
            FullPath = "/path",
            Barcodes = new List<string> { "123" }
        };

        var domain = dto.ToDomain();

        domain.Barcodes.Should().BeAssignableTo<IReadOnlyList<string>>();
    }

    [Fact]
    public void ToDomain_EmptyBarcodes_MapsCorrectly()
    {
        var dto = new MenuItemDto
        {
            Id = "item-456",
            Article = "ART002",
            Name = "Product Without Barcodes",
            Price = 49.50m,
            IsWeighted = false,
            FullPath = "/path",
            Barcodes = new List<string>()
        };

        var domain = dto.ToDomain();

        domain.Barcodes.Should().BeEmpty();
    }

    [Fact]
    public void ToDomain_IsWeightedFalse_MapsCorrectly()
    {
        var dto = new MenuItemDto
        {
            Id = "item-789",
            Article = "ART003",
            Name = "Non-Weighted Product",
            Price = 15.00m,
            IsWeighted = false,
            FullPath = "/category",
            Barcodes = new List<string>()
        };

        var domain = dto.ToDomain();

        domain.IsWeighted.Should().BeFalse();
    }

    [Fact]
    public void ToDomain_Collection_MapsAllItems()
    {
        var dtos = new List<MenuItemDto>
        {
            new MenuItemDto
            {
                Id = "1",
                Article = "A1",
                Name = "Item 1",
                Price = 10m,
                IsWeighted = false,
                FullPath = "/path1",
                Barcodes = new List<string> { "111" }
            },
            new MenuItemDto
            {
                Id = "2",
                Article = "A2",
                Name = "Item 2",
                Price = 20m,
                IsWeighted = true,
                FullPath = "/path2",
                Barcodes = new List<string> { "222", "333" }
            },
            new MenuItemDto
            {
                Id = "3",
                Article = "A3",
                Name = "Item 3",
                Price = 30m,
                IsWeighted = false,
                FullPath = "/path3",
                Barcodes = new List<string>()
            }
        };

        var domainList = dtos.ToDomain();

        domainList.Should().HaveCount(3);
        domainList[0].Id.Should().Be("1");
        domainList[0].Article.Should().Be("A1");
        domainList[0].Price.Should().Be(10m);
        domainList[1].Id.Should().Be("2");
        domainList[1].Article.Should().Be("A2");
        domainList[1].Barcodes.Should().HaveCount(2);
        domainList[2].Id.Should().Be("3");
        domainList[2].Barcodes.Should().BeEmpty();
    }

    [Fact]
    public void ToDomain_Collection_ReturnsReadOnlyList()
    {
        var dtos = new List<MenuItemDto>
        {
            new MenuItemDto
            {
                Id = "1",
                Article = "A1",
                Name = "Item 1",
                Price = 10m,
                IsWeighted = false,
                FullPath = "/path",
                Barcodes = new List<string>()
            }
        };

        var domainList = dtos.ToDomain();

        domainList.Should().BeAssignableTo<IReadOnlyList<SmartMeal.Domain.MenuItem>>();
    }

    [Fact]
    public void ToDomain_EmptyCollection_ReturnsEmptyReadOnlyList()
    {
        var dtos = new List<MenuItemDto>();

        var domainList = dtos.ToDomain();

        domainList.Should().BeEmpty();
        domainList.Should().BeAssignableTo<IReadOnlyList<SmartMeal.Domain.MenuItem>>();
    }

    [Fact]
    public void ToDomain_SpecialCharactersInFields_MapsCorrectly()
    {
        var dto = new MenuItemDto
        {
            Id = "item-special-123",
            Article = "ART@#$",
            Name = "Product with «special» characters: <>&",
            Price = 1234.56m,
            IsWeighted = true,
            FullPath = "/категория/подкатегория/товар",
            Barcodes = new List<string> { "код123", "barcode-456" }
        };

        var domain = dto.ToDomain();

        domain.Id.Should().Be("item-special-123");
        domain.Article.Should().Be("ART@#$");
        domain.Name.Should().Be("Product with «special» characters: <>&");
        domain.FullPath.Should().Be("/категория/подкатегория/товар");
        domain.Barcodes[0].Should().Be("код123");
    }

    [Fact]
    public void ToDomain_VeryLargePrice_MapsCorrectly()
    {
        var dto = new MenuItemDto
        {
            Id = "item-999",
            Article = "EXP",
            Name = "Expensive Item",
            Price = 999999999.99m,
            IsWeighted = false,
            FullPath = "/luxury",
            Barcodes = new List<string>()
        };

        var domain = dto.ToDomain();

        domain.Price.Should().Be(999999999.99m);
    }
}
