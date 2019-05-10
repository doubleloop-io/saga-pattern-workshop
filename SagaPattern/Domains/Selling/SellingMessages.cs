using SagaPattern.Infrastructure;
using System;

namespace SagaPattern.Domains.Selling
{
    public static class SellingMessages
    {
        public class PlaceOrder : CommandBase
        {
            public Guid OrderId { get; set; }
            public Guid ConferenceId { get; set; }
            public int Quantity { get; set; }
        }

        public class BookOrder : CommandBase
        {
            public Guid OrderId { get; set; }
        }

        public class PriceOrder : CommandBase
        {
            public Guid OrderId { get; set; }
            public decimal Amount { get; set; }
        }

        public class SetCustomer : CommandBase
        {
            public Guid OrderId { get; set; }
            public Guid CustomerId { get; set; }
            public Guid BusinessCustomerId { get; set; }
            public Guid PaymentGatewayId { get; set; }
        }

        public class ConfirmOrder : CommandBase
        {
            public Guid OrderId { get; set; }
        }

        public class CancelOrder : CommandBase
        {
            public Guid OrderId { get; set; }
            public string Reason { get; set; }
        }

        public class OrderPlaced : EventBase
        {
            public Guid OrderId { get; set; }
            public Guid ConferenceId { get; set; }
            public int Quantity { get; set; }
        }

        public class OrderBooked : EventBase
        {
            public Guid OrderId { get; set; }
        }

        public class CustomerSet : EventBase
        {
            public Guid OrderId { get; set; }
            public Guid CustomerId { get; set; }
            public Guid BusinessCustomerId { get; set; }
            public Guid PaymentGatewayId { get; set; }
        }

        public class OrderPriced : EventBase
        {
            public Guid OrderId { get; set; }
            public decimal Amount { get; set; }
        }

        public class OrderConfirmed : EventBase
        {
            public Guid OrderId { get; set; }
        }

        public class OrderCanceled : EventBase
        {
            public Guid OrderId { get; set; }
            public string Reason { get; set; }
        }
    }
}