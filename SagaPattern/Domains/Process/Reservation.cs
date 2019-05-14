using System;

namespace SagaPattern.Domains.Process
{
    public class Reservation
    {
        public enum ReservationStatus
        {
            AwaitingReservationConfirmed,
            AwaitingOrderBooked,
            AwaitingPrice,
            AwaitingOrderPriced
        }

        public Reservation(Guid id, Guid conferenceId, int quantity)
        {
            Id = id;
            ConferenceId = conferenceId;
            Quantity = quantity;
            Status = ReservationStatus.AwaitingReservationConfirmed;
        }

        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public ReservationStatus Status { get; set; }
        public Guid ConferenceId { get; private set; }
        public int Quantity { get; private set; }
    }
}