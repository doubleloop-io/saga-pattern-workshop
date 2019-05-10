using SagaPattern.Infrastructure;
using System;

namespace SagaPattern.Domains.Inventory
{
    public static class InventoryMessages
    {
        public class AddSeats : CommandBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public int Quantity { get; set; }
        }

        public class RemoveSeats : CommandBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public int Quantity { get; set; }
        }

        public class MakeSeatsReservation : CommandBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public Guid ReservationId { get; set; }
            public int Quantity { get; set; }
        }

        public class CancelSeatsReservation : CommandBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public Guid ReservationId { get; set; }
        }

        public class CommitSeatsReservation : CommandBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public Guid ReservationId { get; set; }
        }

        public class SeatsAdded : EventBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public int Quantity { get; set; }
        }

        public class SeatsRemoved : EventBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public int Quantity { get; set; }
        }

        public class SeatsReservationAccepted : EventBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public Guid ReservationId { get; set; }
            public int Quantity { get; set; }
        }

        public class SeatsReservationRejected : EventBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public Guid ReservationId { get; set; }
            public int Quantity { get; set; }
            public string Reason { get; set; }
        }

        public class SeatsReservationCanceled : EventBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public Guid ReservationId { get; set; }
        }

        public class SeatsReservationCommitted : EventBase
        {
            public Guid SeatsAvailabilityId { get; set; }
            public Guid ReservationId { get; set; }
        }
    }
}