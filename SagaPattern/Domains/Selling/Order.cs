using System;

namespace SagaPattern.Domains.Selling
{
    public class Order
    {
        public enum OrderStatus
        {
            Placed,
            Booked,
            Priced,
            CustomerSet,
            Completed,
            Canceled
        }

        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public OrderStatus Status { get; private set; }

        public Order(Guid id)
        {
            Id = id;
            Status = OrderStatus.Placed;
        }

        public void Book()
        {
            if (Status != OrderStatus.Placed)
                throw new InvalidOperationException();
            Status = OrderStatus.Booked;
        }

        public void Price()
        {
            if (Status != OrderStatus.Booked)
                throw new InvalidOperationException();
            Status = OrderStatus.Priced;
        }

        public void SetCustomer()
        {
            if (Status != OrderStatus.Priced)
                throw new InvalidOperationException();
            Status = OrderStatus.CustomerSet;
        }

        public void Confirm()
        {
            if (Status != OrderStatus.CustomerSet)
                throw new InvalidOperationException();
            Status = OrderStatus.Completed;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException();
            Status = OrderStatus.Canceled;
        }
    }
}