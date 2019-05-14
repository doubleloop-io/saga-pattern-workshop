using System.Threading.Tasks;
using SagaPattern.Infrastructure;
using static SagaPattern.Domains.Pricing.PricingMessages;

namespace SagaPattern.Domains.Pricing
{
    public class PricingHandler : ICommandHandler<CalculatePrice>
    {
        private readonly PricingService pricingService;
        private readonly IPublisher channel;

        public PricingHandler(PricingService pricingService, IPublisher channel)
        {
            this.pricingService = pricingService;
            this.channel = channel;
        }

        public async Task Handle(CalculatePrice message)
        {
            var price = pricingService.ComputeFor(message.ConferenceId, message.Quantity);

            await channel.Publish(new PriceCalculated
            {
                ReferenceId = message.ReferenceId,
                Price = price
            });
        }
    }
}