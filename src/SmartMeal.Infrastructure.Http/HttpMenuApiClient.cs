using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SmartMeal.Application.Abstractions;
using SmartMeal.Application.Common;
using SmartMeal.Domain;
using SmartMeal.Infrastructure.Http.Dto.Requests;
using SmartMeal.Infrastructure.Http.Dto.Responses;
using SmartMeal.Infrastructure.Http.Mapping;

namespace SmartMeal.Infrastructure.Http;

public sealed class HttpMenuApiClient : IMenuApiClient
{
    private readonly HttpClient _httpClient;

    public HttpMenuApiClient(HttpClient httpClient, HttpClientOptions options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.BaseUrl);

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}"));

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
    }

    public async Task<Result<IReadOnlyList<MenuItem>>> GetMenuAsync(
        bool withPrice,
        CancellationToken cancellationToken = default)
    {
        var request = new ApiRequest<GetMenuParameters>
        {
            Command = "GetMenu",
            CommandParameters = new GetMenuParameters { WithPrice = withPrice }
        };

        try
        {
            var json = JsonSerializer.Serialize(request, JsonConfiguration.DefaultOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<GetMenuData>>(
                responseJson,
                JsonConfiguration.DefaultOptions);

            if (apiResponse is null)
            {
                return Result<IReadOnlyList<MenuItem>>.Fail("Deserialization", "Failed to deserialize response");
            }

            if (!apiResponse.Success)
            {
                return Result<IReadOnlyList<MenuItem>>.Fail("ApiError", apiResponse.ErrorMessage);
            }

            var menuItems = apiResponse.Data.MenuItems.ToDomain();
            return Result<IReadOnlyList<MenuItem>>.Ok(menuItems);
        }
        catch (HttpRequestException ex)
        {
            return Result<IReadOnlyList<MenuItem>>.Fail(Error.FromException(ex));
        }
        catch (JsonException ex)
        {
            return Result<IReadOnlyList<MenuItem>>.Fail(Error.FromException(ex));
        }
    }

    public async Task<Result<Unit>> SendOrderAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        var request = new ApiRequest<SendOrderParameters>
        {
            Command = "SendOrder",
            CommandParameters = new SendOrderParameters
            {
                OrderId = order.Id.ToString(),
                MenuItems = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.MenuItemId,
                    Quantity = item.Quantity.ToString("G29")
                }).ToList()
            }
        };

        try
        {
            var json = JsonSerializer.Serialize(request, JsonConfiguration.DefaultOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(
                responseJson,
                JsonConfiguration.DefaultOptions);

            if (apiResponse is null)
            {
                return Result<Unit>.Fail("Deserialization", "Failed to deserialize response");
            }

            if (!apiResponse.Success)
            {
                return Result<Unit>.Fail("ApiError", apiResponse.ErrorMessage);
            }

            return Result<Unit>.Ok(Unit.Value);
        }
        catch (HttpRequestException ex)
        {
            return Result<Unit>.Fail(Error.FromException(ex));
        }
        catch (JsonException ex)
        {
            return Result<Unit>.Fail(Error.FromException(ex));
        }
    }
}
