using SagaPattern.Infrastructure;
using SagaPattern.Infrastructure.JsonStore;
using static SagaPattern.Domains.Inventory.InventoryMessages;
using static SagaPattern.Domains.Pricing.PricingMessages;
using static SagaPattern.Domains.Selling.SellingMessages;

namespace SagaPattern.Domains.Process
{
    public class ProcessModule
    {
        public static void Bootstrap(IBus bus)
        {
            var reservationRepository = new JsonStore<Reservation>();
            var reservationSaga = new ReservationSaga(reservationRepository, bus);

            bus.Subscribe<OrderPlaced>(reservationSaga);
            bus.Subscribe<SeatsReservationAccepted>(reservationSaga);
            bus.Subscribe<OrderBooked>(reservationSaga);
            bus.Subscribe<PriceCalculated>(reservationSaga);
        }
    }
}