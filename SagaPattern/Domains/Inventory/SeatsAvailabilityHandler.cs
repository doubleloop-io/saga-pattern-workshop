using SagaPattern.Infrastructure;
using System;
using System.Threading.Tasks;
using static SagaPattern.Domains.Inventory.InventoryMessages;

namespace SagaPattern.Domains.Inventory
{
    public class SeatsAvailabilityHandler :
        ICommandHandler<MakeSeatsReservation>,
        ICommandHandler<CancelSeatsReservation>,
        ICommandHandler<CommitSeatsReservation>,
        ICommandHandler<AddSeats>,
        ICommandHandler<RemoveSeats>
    {
        readonly IStore<SeatsAvailability> store;
        readonly IPublisher channel;

        public SeatsAvailabilityHandler(IStore<SeatsAvailability> store, IPublisher channel)
        {
            this.store = store;
            this.channel = channel;
        }

        public async Task Handle(AddSeats command)
        {
            var availability = store.Get(command.SeatsAvailabilityId) ?? 
                               new SeatsAvailability(command.SeatsAvailabilityId);

            availability.AddSeats(command.Quantity);
            store.Save(availability);

            await channel.Publish(new SeatsAdded
            {
                SeatsAvailabilityId = command.SeatsAvailabilityId,
                Quantity = command.Quantity
            });
        }

        public async Task Handle(RemoveSeats command) => 
            await HandleIfSeatsAvailabilityExist(command.SeatsAvailabilityId, command, Handle);

        private async Task Handle(SeatsAvailability availability, RemoveSeats command)
        {
            availability.RemoveSeats(command.Quantity);
            store.Save(availability);

            await channel.Publish(new SeatsRemoved
            {
                SeatsAvailabilityId = command.SeatsAvailabilityId,
                Quantity = command.Quantity
            });
        }

        public async Task Handle(MakeSeatsReservation command) => 
            await HandleIfSeatsAvailabilityExist(command.SeatsAvailabilityId, command, Handle);

        private async Task Handle(SeatsAvailability availability, MakeSeatsReservation command)
        {
            var result = availability.MakeReservation(command.ReservationId, command.Quantity);

            store.Save(availability);

            if (result == SeatsAvailability.MarkResult.Accepted)
            {
                await channel.Publish(new SeatsReservationAccepted
                    {
                        SeatsAvailabilityId = command.SeatsAvailabilityId,
                        ReservationId = command.ReservationId,
                        Quantity = command.Quantity
                    });
            }
            else
            {
                await channel.Publish(new SeatsReservationRejected
                    {
                        SeatsAvailabilityId = command.SeatsAvailabilityId,
                        ReservationId = command.ReservationId,
                        Quantity = command.Quantity,
                        Reason = "No availability."
                    });
            }
        }

        public async Task Handle(CancelSeatsReservation command) => 
            await HandleIfSeatsAvailabilityExist(command.SeatsAvailabilityId, command, Handle);

        private async Task Handle(SeatsAvailability availability, CancelSeatsReservation command)
        {
            availability.CancelReservation(command.ReservationId);
            store.Save(availability);

            await channel.Publish(new SeatsReservationCanceled
            {
                SeatsAvailabilityId = command.SeatsAvailabilityId,
                ReservationId = command.ReservationId
            });
        }

        public async Task Handle(CommitSeatsReservation command) => 
            await HandleIfSeatsAvailabilityExist(command.SeatsAvailabilityId, command, Handle);

        private async Task Handle(SeatsAvailability availability, CommitSeatsReservation command)
        {
            availability.CommitReservation(command.ReservationId);
            store.Save(availability);

            await channel.Publish(new SeatsReservationCommitted
            {
                SeatsAvailabilityId = command.SeatsAvailabilityId,
                ReservationId = command.ReservationId
            });
        }

        private async Task HandleIfSeatsAvailabilityExist<TCommand>(Guid seatsAvailabilityId, TCommand command, Func<SeatsAvailability, TCommand, Task> handle)
        {
                var availability = store.Get(seatsAvailabilityId);

                if (availability == null)
                {
                    throw new InvalidOperationException($"Seats availability [{seatsAvailabilityId}] does not exists");
                }
                await handle(availability, command);
        }
    }
}