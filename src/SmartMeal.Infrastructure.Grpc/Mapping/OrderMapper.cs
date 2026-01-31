using SmartMeal.Domain;
using GrpcOrder = Sms.Test.Order;
using GrpcOrderItem = Sms.Test.OrderItem;

namespace SmartMeal.Infrastructure.Grpc.Mapping;

public static class OrderMapper
{
    public static GrpcOrder ToGrpc(this Order order)
    {
        var grpcOrder = new GrpcOrder
        {
            Id = order.Id.ToString()
        };

        foreach (var item in order.Items)
        {
            grpcOrder.OrderItems.Add(new GrpcOrderItem
            {
                Id = item.MenuItemId,
                Quantity = (double)item.Quantity
            });
        }

        return grpcOrder;
    }
}
