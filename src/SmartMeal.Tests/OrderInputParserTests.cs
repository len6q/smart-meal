using FluentAssertions;
using SmartMeal.Console;

namespace SmartMeal.Tests;

public sealed class OrderInputParserTests
{
    [Fact]
    public void Parse_ValidInput_ReturnsSuccessWithCorrectItems()
    {
        var input = "A1:1;A2:2,5";

        var result = OrderInputParser.Parse(input);

        result.Should().BeOfType<SmartMeal.Application.Common.Result<IReadOnlyList<(string Article, decimal Quantity)>>.Success>();

        var items = result.Match(
            onSuccess: value => value,
            onFailure: _ => throw new InvalidOperationException("Expected success"));

        items.Should().HaveCount(2);
        items[0].Article.Should().Be("A1");
        items[0].Quantity.Should().Be(1m);
        items[1].Article.Should().Be("A2");
        items[1].Quantity.Should().Be(2.5m);
    }

    [Fact]
    public void Parse_EmptyString_ReturnsFailure()
    {
        var input = string.Empty;

        var result = OrderInputParser.Parse(input);

        result.Should().BeOfType<SmartMeal.Application.Common.Result<IReadOnlyList<(string Article, decimal Quantity)>>.Failure>();

        var error = result.Match(
            onSuccess: _ => throw new InvalidOperationException("Expected failure"),
            onFailure: error => error);

        error.Message.Should().Be("Ввод не может быть пустым");
    }

    [Fact]
    public void Parse_WhitespaceString_ReturnsFailure()
    {
        var input = "   ";

        var result = OrderInputParser.Parse(input);

        result.Should().BeOfType<SmartMeal.Application.Common.Result<IReadOnlyList<(string Article, decimal Quantity)>>.Failure>();

        var error = result.Match(
            onSuccess: _ => throw new InvalidOperationException("Expected failure"),
            onFailure: error => error);

        error.Message.Should().Be("Ввод не может быть пустым");
    }

    [Fact]
    public void Parse_InvalidFormat_MissingColon_ReturnsFailure()
    {
        var input = "A1-1;A2:2";

        var result = OrderInputParser.Parse(input);

        result.Should().BeOfType<SmartMeal.Application.Common.Result<IReadOnlyList<(string Article, decimal Quantity)>>.Failure>();

        var error = result.Match(
            onSuccess: _ => throw new InvalidOperationException("Expected failure"),
            onFailure: error => error);

        error.Message.Should().Contain("Неверный формат позиции");
    }

    [Fact]
    public void Parse_InvalidFormat_InvalidQuantity_ReturnsFailure()
    {
        var input = "A1:abc";

        var result = OrderInputParser.Parse(input);

        result.Should().BeOfType<SmartMeal.Application.Common.Result<IReadOnlyList<(string Article, decimal Quantity)>>.Failure>();

        var error = result.Match(
            onSuccess: _ => throw new InvalidOperationException("Expected failure"),
            onFailure: error => error);

        error.Message.Should().Contain("Неверное количество для");
    }

    [Fact]
    public void Parse_ZeroQuantity_ReturnsFailure()
    {
        var input = "A1:0";

        var result = OrderInputParser.Parse(input);

        result.Should().BeOfType<SmartMeal.Application.Common.Result<IReadOnlyList<(string Article, decimal Quantity)>>.Failure>();

        var error = result.Match(
            onSuccess: _ => throw new InvalidOperationException("Expected failure"),
            onFailure: error => error);

        error.Message.Should().Contain("должно быть больше нуля");
    }

    [Fact]
    public void Parse_NegativeQuantity_ReturnsFailure()
    {
        var input = "A1:-5";

        var result = OrderInputParser.Parse(input);

        result.Should().BeOfType<SmartMeal.Application.Common.Result<IReadOnlyList<(string Article, decimal Quantity)>>.Failure>();

        var error = result.Match(
            onSuccess: _ => throw new InvalidOperationException("Expected failure"),
            onFailure: error => error);

        error.Message.Should().Contain("должно быть больше нуля");
    }

    [Fact]
    public void Parse_MultipleItems_AllValid_ReturnsSuccess()
    {
        var input = "PROD1:10;PROD2:3,5;PROD3:1,25";

        var result = OrderInputParser.Parse(input);

        result.Should().BeOfType<SmartMeal.Application.Common.Result<IReadOnlyList<(string Article, decimal Quantity)>>.Success>();

        var items = result.Match(
            onSuccess: value => value,
            onFailure: _ => throw new InvalidOperationException("Expected success"));

        items.Should().HaveCount(3);
        items[0].Should().Be(("PROD1", 10m));
        items[1].Should().Be(("PROD2", 3.5m));
        items[2].Should().Be(("PROD3", 1.25m));
    }
}
