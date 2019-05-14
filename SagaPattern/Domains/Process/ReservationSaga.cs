using System;
using System.Threading.Tasks;
using SagaPattern.Infrastructure;
using static SagaPattern.Domains.Inventory.InventoryMessages;
using static SagaPattern.Domains.Pricing.PricingMessages;
using static SagaPattern.Domains.Selling.SellingMessages;

namespace SagaPattern.Domains.Process
{
    public class ReservationSaga : IEventHandler<OrderPlaced>,
        IEventHandler<SeatsReservationAccepted>,
        IEventHandler<OrderBooked>,
        IEventHandler<PriceCalculated>
    {
        private readonly IStore<Reservation> store;
        private readonly IPublisher channel;

        public ReservationSaga(IStore<Reservation> store, IPublisher channel)
        {
            this.store = store;
            this.channel = channel;
        }

        public async Task Handle(OrderPlaced message)
        {
            var reservation = store.Get(message.OrderId);

            if (reservation != null)
            {
                throw new InvalidOperationException($"Reservation {message.OrderId} already started");
            }
            reservation = new Reservation(message.OrderId, message.ConferenceId, message.Quantity);
            store.Save(reservation);

            await channel.Publish(new MakeSeatsReservation
            {
                ReservationId = reservation.Id,
                SeatsAvailabilityId = message.ConferenceId,
                Quantity = message.Quantity
            });
        }

        public async Task Handle(SeatsReservationAccepted message) => 
            await ExecuteIfReservationExists(message.ReservationId, message, Handle);

        private async Task Handle(Reservation reservation, SeatsReservationAccepted message)
        {
            if (reservation.Status != Reservation.ReservationStatus.AwaitingReservationConfirmed)
            {
                throw new InvalidOperationException($"Reservation {reservation.Id} - cannot manage seats reservation");
            }
            reservation.Status = Reservation.ReservationStatus.AwaitingOrderBooked;
            store.Save(reservation);
            await channel.Publish(new BookOrder
            {
                OrderId = reservation.Id
            });
        }

        public Task Handle(OrderBooked message) =>
            ExecuteIfReservationExists(message.OrderId, message, Handle);

        private async Task Handle(Reservation reservation, OrderBooked message)
        {
            if (reservation.Status != Reservation.ReservationStatus.AwaitingOrderBooked)
            {
                throw new InvalidOperationException($"Reservation {reservation.Id} - cannot manage order booked");
            }
            reservation.Status = Reservation.ReservationStatus.AwaitingPrice;
            store.Save(reservation);
            await channel.Publish(new CalculatePrice
            {
                ReferenceId = reservation.Id,
                ConferenceId = reservation.ConferenceId,
                Quantity = reservation.Quantity
            });
        }

        public Task Handle(PriceCalculated message) =>
            ExecuteIfReservationExists(message.ReferenceId, message, Handle);

        private async Task Handle(Reservation reservation, PriceCalculated message)
        {
            if (reservation.Status != Reservation.ReservationStatus.AwaitingPrice)
            {
                throw new InvalidOperationException($"Reservation {reservation.Id} - cannot manage price");
            }
            reservation.Status = Reservation.ReservationStatus.AwaitingOrderPriced;
            store.Save(reservation);
            await channel.Publish(new PriceOrder
            {
                OrderId = reservation.Id,
                Amount = message.Price
            });
        }

        private async Task ExecuteIfReservationExists<T>(Guid reservationId, T message, Func<Reservation, T, Task> func)
        {
            var reservation = store.Get(reservationId);

            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation {reservationId} does not exist");
            }
            await func(reservation, message);
        }
    }
}