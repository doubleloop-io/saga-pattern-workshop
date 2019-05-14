using System;

namespace SagaPattern.Domains.Pricing
{
    public class PricingService
    {
        public decimal ComputeFor(Guid conferenceId, int quantity)
        {
            var unitPrice = conferenceId == Ids.LambdaWorld ? 20 : 100;
            return unitPrice * quantity;
        }
    }
}