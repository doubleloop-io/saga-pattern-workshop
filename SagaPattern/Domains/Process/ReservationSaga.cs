using System;
using System.Threading.Tasks;
using SagaPattern.Infrastructure;
using static SagaPattern.Domains.Inventory.InventoryMessages;
using static SagaPattern.Domains.Payment.PaymentMessages;
using static SagaPattern.Domains.Pricing.PricingMessages;
using static SagaPattern.Domains.Selling.SellingMessages;

namespace SagaPattern.Domains.Process
{
    public class ReservationSaga : IEventHandler<OrderPlaced>,
        IEventHandler<SeatsReservationAccepted>,
        IEventHandler<OrderBooked>,
        IEventHandler<PriceCalculated>,
        IEventHandler<OrderPriced>,
        IEventHandler<CustomerSet>,
        IEventHandler<PaymentAccepted>,
        IEventHandler<SeatsReservationCommitted>
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

        public Task Handle(OrderPriced message) =>
            ExecuteIfReservationExists(message.OrderId, message, Handle);

        private Task Handle(Reservation reservation, OrderPriced message)
        {
            if (reservation.Status != Reservation.ReservationStatus.AwaitingOrderPriced)
            {
                throw new InvalidOperationException($"Reservation {reservation.Id} - cannot order priced");
            }

            reservation.Status = Reservation.ReservationStatus.AwaitingOrderCustomerSet;
            reservation.Amount = message.Amount;
            store.Save(reservation);

            return Task.CompletedTask;
        }

        public Task Handle(CustomerSet message) =>
            ExecuteIfReservationExists(message.OrderId, message, Handle);

        private async Task Handle(Reservation reservation, CustomerSet message)
        {
            if (reservation.Status != Reservation.ReservationStatus.AwaitingOrderCustomerSet)
            {
                throw new InvalidOperationException($"Reservation {reservation.Id} - cannot customer set");
            }

            reservation.Status = Reservation.ReservationStatus.AwaitingPaymentAccepted;
            store.Save(reservation);

            await channel.Publish(new MakePayment
            {
                ReferenceId = reservation.Id,
                ConferenceId = reservation.ConferenceId,
                Amount = reservation.Amount,
                BusinessCustomerId = message.BusinessCustomerId,
                PaymentGatewayId = message.PaymentGatewayId
            });
        }

        public Task Handle(PaymentAccepted message) =>
            ExecuteIfReservationExists(message.ReferenceId, message, Handle);

        private async Task Handle(Reservation reservation, PaymentAccepted message)
        {
            if (reservation.Status != Reservation.ReservationStatus.AwaitingPaymentAccepted)
            {
                throw new InvalidOperationException($"Reservation {reservation.Id} - cannot manage payment accepted");
            }

            reservation.Status = Reservation.ReservationStatus.AwaitingReservationCommitted;
            store.Save(reservation);

            await channel.Publish(new CommitSeatsReservation
            {
                ReservationId = reservation.Id,
                SeatsAvailabilityId = reservation.ConferenceId
            });
        }

        public Task Handle(SeatsReservationCommitted message) =>
            ExecuteIfReservationExists(message.ReservationId, message, Handle);

        private async Task Handle(Reservation reservation, SeatsReservationCommitted message)
        {
            if (reservation.Status != Reservation.ReservationStatus.AwaitingReservationCommitted)
            {
                throw new InvalidOperationException($"Reservation {reservation.Id} - cannot seats reservation committed");
            }

            reservation.Status = Reservation.ReservationStatus.Completed;
            store.Save(reservation);

            await channel.Publish(new ConfirmOrder
            {
                OrderId = reservation.Id
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