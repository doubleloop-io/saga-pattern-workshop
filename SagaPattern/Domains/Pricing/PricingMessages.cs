using System;
using SagaPattern.Infrastructure;

namespace SagaPattern.Domains.Pricing
{
    public static class PricingMessages
    {
        public class CalculatePrice : CommandBase
        {
            public Guid ReferenceId { get; set; }
            public Guid ConferenceId { get; set; }
            public int Quantity { get; set; }
        }

        public class PriceCalculated : EventBase
        {
            public Guid ReferenceId { get; set; }
            public decimal Price { get; set; }
        }
    }
}