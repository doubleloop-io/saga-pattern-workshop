using SagaPattern.Infrastructure;
using SagaPattern.Infrastructure.JsonStore;
using static SagaPattern.Domains.Selling.SellingMessages;

namespace SagaPattern.Domains.Selling
{
    public static class SellingModule
    {
        public static void Bootstrap(IBus bus)
        {
            var orderRepository = new JsonStore<Order>();
            var orderHandler = new OrderHandler(orderRepository, bus);

            bus.Subscribe<PlaceOrder>(orderHandler);
            bus.Subscribe<SetCustomer>(orderHandler);
            bus.Subscribe<BookOrder>(orderHandler);
            bus.Subscribe<PriceOrder>(orderHandler);
            bus.Subscribe<ConfirmOrder>(orderHandler);
            bus.Subscribe<CancelOrder>(orderHandler);

            bus.Subscribe(new ReserveSeatOnOrderPlacedPolicy(bus));
        }
    }
}