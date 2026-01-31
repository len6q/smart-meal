using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using SmartMeal.Application.Abstractions;
using SmartMeal.Application.Common;
using SmartMeal.Infrastructure.Grpc.Mapping;
using Sms.Test;
using DomainMenuItem = SmartMeal.Domain.MenuItem;
using DomainOrder = SmartMeal.Domain.Order;

namespace SmartMeal.Infrastructure.Grpc;

public sealed class GrpcMenuApiClient : IMenuApiClient
{
    private readonly SmsTestService.SmsTestServiceClient _client;

    public GrpcMenuApiClient(GrpcClientOptions options)
    {
        var channel = GrpcChannel.ForAddress(options.Address);
        _client = new SmsTestService.SmsTestServiceClient(channel);
    }

    public async Task<Result<IReadOnlyList<DomainMenuItem>>> GetMenuAsync(
        bool withPrice,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new BoolValue { Value = withPrice };
            var response = await _client.GetMenuAsync(request, cancellationToken: cancellationToken);

            if (!response.Success)
            {
                return Result<IReadOnlyList<DomainMenuItem>>.Fail("GrpcError", response.ErrorMessage);
            }

            var menuItems = response.MenuItems.ToDomain();
            return Result<IReadOnlyList<DomainMenuItem>>.Ok(menuItems);
        }
        catch (RpcException ex)
        {
            return Result<IReadOnlyList<DomainMenuItem>>.Fail(Error.FromException(ex));
        }
    }

    public async Task<Result<Unit>> SendOrderAsync(
        DomainOrder order,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var grpcOrder = order.ToGrpc();
            var response = await _client.SendOrderAsync(grpcOrder, cancellationToken: cancellationToken);

            if (!response.Success)
            {
                return Result<Unit>.Fail("GrpcError", response.ErrorMessage);
            }

            return Result<Unit>.Ok(Unit.Value);
        }
        catch (RpcException ex)
        {
            return Result<Unit>.Fail(Error.FromException(ex));
        }
    }
}
