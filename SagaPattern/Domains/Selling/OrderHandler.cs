using SagaPattern.Infrastructure;
using System;
using System.Threading.Tasks;
using static SagaPattern.Domains.Selling.SellingMessages;

namespace SagaPattern.Domains.Selling
{
    public class OrderHandler :
        ICommandHandler<PlaceOrder>,
        ICommandHandler<SetCustomer>,
        ICommandHandler<BookOrder>,
        ICommandHandler<PriceOrder>,
        ICommandHandler<ConfirmOrder>,
        ICommandHandler<CancelOrder>
    {
        readonly IStore<Order> store;
        readonly IPublisher channel;

        public OrderHandler(IStore<Order> store, IPublisher channel)
        {
            this.store = store;
            this.channel = channel;
        }

        public async Task Handle(PlaceOrder command)
        {
            if (store.Get(command.OrderId) != null)
            {
                throw new InvalidOperationException($"Order [{command.OrderId}] already exists");
            }

            var order = new Order(command.OrderId);
            store.Save(order);

            await channel.Publish(new OrderPlaced
            {
                OrderId = order.Id,
                ConferenceId = command.ConferenceId,
                Quantity = command.Quantity
            });
        }

        public Task Handle(BookOrder command) => 
            HandleIfOrderExist(command.OrderId, command, Handle);

        private async Task Handle(Order order, BookOrder command)
        {
            order.Book();
            store.Save(order);

            await channel.Publish(new OrderBooked
            {
                OrderId = order.Id
            });
        }

        public Task Handle(PriceOrder command) => 
            HandleIfOrderExist(command.OrderId, command, Handle);

        private async Task Handle(Order order, PriceOrder command)
        {
            order.Price();
            store.Save(order);

            await channel.Publish(new OrderPriced
            {
                OrderId = order.Id,
                Amount = command.Amount
            });
        }

        public Task Handle(SetCustomer command) =>
            HandleIfOrderExist(command.OrderId, command, Handle);

        private async Task Handle(Order order, SetCustomer command)
        {
            order.SetCustomer();
            store.Save(order);

            await channel.Publish(new CustomerSet
            {
                OrderId = order.Id,
                CustomerId = command.CustomerId,
                BusinessCustomerId = command.BusinessCustomerId,
                PaymentGatewayId = command.PaymentGatewayId
            });
        }

        public Task Handle(ConfirmOrder command) =>
            HandleIfOrderExist(command.OrderId, command, Handle);

        private async Task Handle(Order order, ConfirmOrder command)
        {
            order.Confirm();
            store.Save(order);

            await channel.Publish(new OrderConfirmed
            {
                OrderId = order.Id
            });
        }

        public Task Handle(CancelOrder command) =>
            HandleIfOrderExist(command.OrderId, command, Handle);

        private async Task Handle(Order order, CancelOrder command)
        {
            order.Cancel();
            store.Save(order);

            await channel.Publish(new OrderCanceled
            {
                OrderId = order.Id,
                Reason = command.Reason
            });
        }

        private async Task HandleIfOrderExist<TCommand>(Guid orderId, TCommand command, Func<Order, TCommand, Task> handle)
        {
            var order = store.Get(orderId);

            if (order == null)
            {
                throw new InvalidOperationException($"Order [{orderId}] does not exists");
            }
            await handle(order, command);
        }
    }
}