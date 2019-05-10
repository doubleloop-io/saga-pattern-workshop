using SagaPattern.Infrastructure;
using SagaPattern.Infrastructure.JsonStore;
using static SagaPattern.Domains.Inventory.InventoryMessages;

namespace SagaPattern.Domains.Inventory
{
    public static class InventoryModule
    {
        public static void Bootstrap(IBus bus)
        {
            var seatAvailabilityRepository = new JsonStore<SeatsAvailability>();
            var seatsAvailabilityHandler = new SeatsAvailabilityHandler(seatAvailabilityRepository, bus);

            bus.Subscribe<AddSeats>(seatsAvailabilityHandler);
            bus.Subscribe<RemoveSeats>(seatsAvailabilityHandler);
            bus.Subscribe<MakeSeatsReservation>(seatsAvailabilityHandler);
            bus.Subscribe<CancelSeatsReservation>(seatsAvailabilityHandler);
            bus.Subscribe<CommitSeatsReservation>(seatsAvailabilityHandler);
        }

    }
}