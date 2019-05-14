using System.Threading.Tasks;
using SagaPattern.Domains.Inventory;
using SagaPattern.Infrastructure;
using static SagaPattern.Domains.Selling.SellingMessages;

namespace SagaPattern.Domains.Selling
{
    public class ReserveSeatOnOrderPlacedPolicy : IEventHandler<OrderPlaced>
    {
        private readonly IPublisher channel;

        public ReserveSeatOnOrderPlacedPolicy(IPublisher channel)
        {
            this.channel = channel;
        }

        public async Task Handle(OrderPlaced message)
        {
            await channel.Publish(new InventoryMessages.MakeSeatsReservation
            {
                ReservationId = message.OrderId,
                SeatsAvailabilityId = message.ConferenceId,
                Quantity = message.Quantity
            });
        }
    }
}