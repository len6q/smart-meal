using SmartMeal.Application.Common;
using SmartMeal.Domain;

namespace SmartMeal.Application.Abstractions;

public interface IMenuApiClient
{
    Task<Result<IReadOnlyList<MenuItem>>> GetMenuAsync(
        bool withPrice,
        CancellationToken cancellationToken = default);

    Task<Result<Unit>> SendOrderAsync(
        Order order,
        CancellationToken cancellationToken = default);
}
