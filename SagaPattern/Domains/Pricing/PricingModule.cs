using SagaPattern.Infrastructure;

namespace SagaPattern.Domains.Pricing
{
    public static class PricingModule
    {
        public static void Bootstrap(IBus bus)
        {
            var pricingService = new PricingService();
            var pricingHandler = new PricingHandler(pricingService, bus);

            bus.Subscribe(pricingHandler);
        }
    }
}