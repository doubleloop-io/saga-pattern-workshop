﻿using SagaPattern.Infrastructure;
using SagaPattern.Infrastructure.JsonStore;
using static SagaPattern.Domains.Inventory.InventoryMessages;
using static SagaPattern.Domains.Payment.PaymentMessages;
using static SagaPattern.Domains.Pricing.PricingMessages;
using static SagaPattern.Domains.Process.ProcessMessages;
using static SagaPattern.Domains.Selling.SellingMessages;

namespace SagaPattern.Domains.Process
{
    public static class ProcessModule
    {
        public static void Bootstrap(IBus bus)
        {
            var reservationRepository = new JsonStore<Reservation>();
            var reservationSaga = new ReservationSaga(reservationRepository, bus);

            bus.Subscribe<OrderPlaced>(reservationSaga);
            bus.Subscribe<SeatsReservationAccepted>(reservationSaga);
            bus.Subscribe<OrderBooked>(reservationSaga);
            bus.Subscribe<PriceCalculated>(reservationSaga);
            bus.Subscribe<OrderPriced>(reservationSaga);
            bus.Subscribe<CustomerSet>(reservationSaga);
            bus.Subscribe<PaymentAccepted>(reservationSaga);
            bus.Subscribe<SeatsReservationCommitted>(reservationSaga);
            bus.Subscribe<PaymentRejected>(reservationSaga);
            bus.Subscribe<SeatsReservationCanceled>(reservationSaga);
            bus.Subscribe<ReservationExpired>(reservationSaga);
        }
    }
}