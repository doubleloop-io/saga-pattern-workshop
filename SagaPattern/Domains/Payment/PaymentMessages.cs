using SagaPattern.Infrastructure;
using System;

namespace SagaPattern.Domains.Payment
{
    public static class PaymentMessages
    {
        public class MakePayment : CommandBase
        {
            public Guid ReferenceId { get; set; }
            public Guid ConferenceId { get; set; }
            public Guid BusinessCustomerId { get; set; }
            public Guid PaymentGatewayId { get; set; }
            public decimal Amount { get; set; }
        }

        public class PaymentAccepted : EventBase
        {
            public Guid ReferenceId { get; set; }
            public Guid ConferenceId { get; set; }
            public Guid BusinessCustomerId { get; set; }
            public Guid PaymentGatewayId { get; set; }
            public decimal Amount { get; set; }
        }

        public class PaymentRejected : EventBase
        {
            public Guid ReferenceId { get; set; }
            public Guid ConferenceId { get; set; }
            public Guid BusinessCustomerId { get; set; }
            public Guid PaymentGatewayId { get; set; }
            public decimal Amount { get; set; }
            public string Reason { get; set; }
        }
    }
}