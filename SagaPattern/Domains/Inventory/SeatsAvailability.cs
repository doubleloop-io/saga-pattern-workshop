using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaPattern.Domains.Inventory
{
    public class SeatsAvailability
    {
        public enum MarkResult
        {
            Accepted,
            Rejected
        }

        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public Dictionary<Guid, int> PendingSeats { get; private set; }
        public Dictionary<Guid, int> TakenSeats { get; private set; }
        public int Quantity { get; private set; }

        public SeatsAvailability(Guid id)
        {
            Id = id;
            PendingSeats = new Dictionary<Guid, int>();
            TakenSeats = new Dictionary<Guid, int>();
        }

        public void AddSeats(int quantity)
        {
            Quantity += quantity;
        }

        public void RemoveSeats(int quantity)
        {
            // NOTE: race condition, suppose you:
            //  1) add 10 seats
            //  1) some one buy 8 of them
            //  3) then remove 5 seats.
            // what do you do? throw exception? refound removed seats? quit your job? :-) 
            // ask to the business people
            Quantity -= quantity;
        }

        public MarkResult MakeReservation(Guid reservationId, int quantity)
        {
            // NOTE: integrity guard, aka programmer failure
            if (ContainsReservation(reservationId))
                throw new InvalidOperationException("Can't reserve more seats for an already processed reservation.");

            var free = Quantity - AllPending() - AllTaken();
            var isAccepted = free >= quantity;
            if (isAccepted)
                PendingSeats.Add(reservationId, quantity);

            return isAccepted ? MarkResult.Accepted : MarkResult.Rejected;
        }

        public void CommitReservation(Guid reservationId)
        {
            if (!PendingSeats.ContainsKey(reservationId))
                throw new InvalidOperationException("Can't commit an unknown reservation");

            var quantity = PendingSeats[reservationId];
            PendingSeats.Remove(reservationId);
            TakenSeats.Add(reservationId, quantity);
        }

        public void CancelReservation(Guid reservationId)
        {
            if (!PendingSeats.ContainsKey(reservationId))
                throw new InvalidOperationException("Can't cancel an unknown reservation");

            PendingSeats.Remove(reservationId);
        }

        bool ContainsReservation(Guid reservationId)
        {
            return new[] { PendingSeats, TakenSeats }.Any(x => x.ContainsKey(reservationId));
        }

        int AllPending()
        {
            return PendingSeats.Values.Sum();
        }

        int AllTaken()
        {
            return TakenSeats.Values.Sum();
        }
    }
}