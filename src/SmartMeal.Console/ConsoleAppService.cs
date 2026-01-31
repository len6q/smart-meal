using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartMeal.Application.Abstractions;
using SmartMeal.Infrastructure.Persistence;

namespace SmartMeal.Console;

public sealed class ConsoleAppService : IHostedService
{
    private readonly ILogger<ConsoleAppService> _logger;
    private readonly IMenuApiClient _apiClient;
    private readonly IMenuRepository _repository;
    private readonly DatabaseInitializer _databaseInitializer;
    private readonly IHostApplicationLifetime _lifetime;

    public ConsoleAppService(
        ILogger<ConsoleAppService> logger,
        IMenuApiClient apiClient,
        IMenuRepository repository,
        DatabaseInitializer databaseInitializer,
        IHostApplicationLifetime lifetime)
    {
        _logger = logger;
        _apiClient = apiClient;
        _repository = repository;
        _databaseInitializer = databaseInitializer;
        _lifetime = lifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Приложение запущено");

            await InitializeDatabaseAsync(cancellationToken);

            var menuItems = await GetMenuFromApiAsync(cancellationToken);
            if (menuItems is null)
            {
                return;
            }

            await SaveAndDisplayMenuAsync(menuItems, cancellationToken);

            var order = CreateOrder();

            var orderItems = await ReadOrderItemsFromUserAsync(menuItems, cancellationToken);
            if (orderItems is null)
            {
                return;
            }

            await SendOrderAsync(order, orderItems, cancellationToken);

            _logger.LogInformation("Приложение завершено успешно");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при выполнении приложения");
        }
        finally
        {
            _lifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Приложение останавливается");
        return Task.CompletedTask;
    }

    private async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _databaseInitializer.InitializeAsync(cancellationToken);
            _logger.LogInformation("База данных инициализирована успешно");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации базы данных");
            throw;
        }
    }

    private async Task<IReadOnlyList<Domain.MenuItem>?> GetMenuFromApiAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос меню от сервера...");

        var result = await _apiClient.GetMenuAsync(withPrice: true, cancellationToken);

        return result.Match<IReadOnlyList<Domain.MenuItem>?>(
            onSuccess: menuItems =>
            {
                _logger.LogInformation("Меню успешно получено от сервера");
                return menuItems;
            },
            onFailure: error =>
            {
                _logger.LogError("Ошибка при получении меню: {ErrorMessage}", error.Message);
                System.Console.WriteLine($"Ошибка: {error.Message}");
                return null;
            });
    }

    private async Task SaveAndDisplayMenuAsync(
        IReadOnlyList<Domain.MenuItem> menuItems,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Сохранение меню в базу данных...");
        await _repository.SaveMenuItemsAsync(menuItems, cancellationToken);
        _logger.LogInformation("Сохранено позиций в БД: {Count}", menuItems.Count);

        System.Console.WriteLine();
        System.Console.WriteLine("Список блюд:");
        System.Console.WriteLine();

        foreach (var item in menuItems)
        {
            System.Console.WriteLine($"{item.Name} – {item.Article} – {item.Price}");
        }

        System.Console.WriteLine();
        _logger.LogInformation("Выведено позиций в консоль: {Count}", menuItems.Count);
    }

    private Domain.Order CreateOrder()
    {
        var orderId = Guid.NewGuid();
        _logger.LogInformation("Создан новый заказ с ID: {OrderId}", orderId);

        return new Domain.Order
        {
            Id = orderId,
            Items = Array.Empty<Domain.OrderItem>()
        };
    }

    private async Task<IReadOnlyList<Domain.OrderItem>?> ReadOrderItemsFromUserAsync(
        IReadOnlyList<Domain.MenuItem> menuItems,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Введите список позиций для заказа в формате:");
            System.Console.WriteLine("Код1:Количество1;Код2:Количество2;...");
            System.Console.WriteLine("Пример: A1004292:2;A1004293:0.5");
            System.Console.Write("> ");

            var input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                System.Console.WriteLine("Ошибка: Ввод не может быть пустым");
                continue;
            }

            var parseResult = OrderInputParser.Parse(input);

            var parsedItems = parseResult.Match<IReadOnlyList<(string Article, decimal Quantity)>?>(
                onSuccess: items => items,
                onFailure: error =>
                {
                    System.Console.WriteLine($"Ошибка: {error.Message}");
                    return null;
                });

            if (parsedItems is null)
            {
                continue;
            }

            var validationResult = await ValidateOrderItemsAsync(parsedItems, cancellationToken);

            if (validationResult is null)
            {
                continue;
            }

            _logger.LogInformation("Заказ успешно введен и валидирован: {Count} позиций", validationResult.Count);
            return validationResult;
        }

        return null;
    }

    private async Task<IReadOnlyList<Domain.OrderItem>?> ValidateOrderItemsAsync(
        IReadOnlyList<(string Article, decimal Quantity)> items,
        CancellationToken cancellationToken)
    {
        var orderItems = new List<Domain.OrderItem>();

        foreach (var (article, quantity) in items)
        {
            var menuItem = await _repository.GetByArticleAsync(article, cancellationToken);

            if (menuItem is null)
            {
                System.Console.WriteLine($"Ошибка: Код '{article}' не найден в базе данных");
                return null;
            }

            orderItems.Add(new Domain.OrderItem
            {
                MenuItemId = menuItem.Id,
                Quantity = quantity
            });
        }

        return orderItems.AsReadOnly();
    }

    private async Task SendOrderAsync(
        Domain.Order order,
        IReadOnlyList<Domain.OrderItem> orderItems,
        CancellationToken cancellationToken)
    {
        var orderWithItems = order with { Items = orderItems };

        _logger.LogInformation("Отправка заказа на сервер...");

        var result = await _apiClient.SendOrderAsync(orderWithItems, cancellationToken);

        result.Match(
            onSuccess: _ =>
            {
                _logger.LogInformation("Заказ успешно отправлен");
                System.Console.WriteLine();
                System.Console.WriteLine("УСПЕХ");
                System.Console.WriteLine();
                return Application.Common.Unit.Value;
            },
            onFailure: error =>
            {
                _logger.LogError("Ошибка при отправке заказа: {ErrorMessage}", error.Message);
                System.Console.WriteLine();
                System.Console.WriteLine($"Ошибка: {error.Message}");
                System.Console.WriteLine();
                return Application.Common.Unit.Value;
            });
    }
}
