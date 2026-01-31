using SmartMeal.Application.Common;

namespace SmartMeal.Console;

public static class OrderInputParser
{
    public static Result<IReadOnlyList<(string Article, decimal Quantity)>> Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return Result<IReadOnlyList<(string Article, decimal Quantity)>>.Fail(
                "ValidationError",
                "Ввод не может быть пустым");
        }

        var items = new List<(string Article, decimal Quantity)>();
        var parts = input.Split(';', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
        {
            return Result<IReadOnlyList<(string Article, decimal Quantity)>>.Fail(
                "ValidationError",
                "Не найдено ни одной позиции");
        }

        foreach (var part in parts)
        {
            var itemParts = part.Split(':', StringSplitOptions.RemoveEmptyEntries);

            if (itemParts.Length != 2)
            {
                return Result<IReadOnlyList<(string Article, decimal Quantity)>>.Fail(
                    "ValidationError",
                    $"Неверный формат позиции: '{part}'. Ожидается 'Код:Количество'");
            }

            var article = itemParts[0].Trim();
            var quantityString = itemParts[1].Trim();

            if (!decimal.TryParse(quantityString, out var quantity))
            {
                return Result<IReadOnlyList<(string Article, decimal Quantity)>>.Fail(
                    "ValidationError",
                    $"Неверное количество для '{article}': '{quantityString}'");
            }

            if (quantity <= 0)
            {
                return Result<IReadOnlyList<(string Article, decimal Quantity)>>.Fail(
                    "ValidationError",
                    $"Количество для '{article}' должно быть больше нуля");
            }

            items.Add((article, quantity));
        }

        return Result<IReadOnlyList<(string Article, decimal Quantity)>>.Ok(items.AsReadOnly());
    }
}
