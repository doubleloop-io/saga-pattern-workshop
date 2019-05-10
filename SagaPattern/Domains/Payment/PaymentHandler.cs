using System;
using System.Threading.Tasks;
using SagaPattern.Infrastructure;
using static SagaPattern.Domains.Payment.PaymentMessages;

namespace SagaPattern.Domains.Payment
{
    public class PaymentHandler : ICommandHandler<MakePayment>
    {
        readonly PaymentGatewaysRegistry registry;
        readonly IPublisher bus;

        public PaymentHandler(PaymentGatewaysRegistry registry, IPublisher bus)
        {
            this.registry = registry;
            this.bus = bus;
        }

        public async Task Handle(MakePayment command)
        {
            var gateway = registry.For(command.PaymentGatewayId);
            try
            {
                var result = await gateway.PayAsync(new PaymentRequest
                {
                    Customer = command.BusinessCustomerId.ToString("N"),
                    Amount = command.Amount
                });

                if (result.Result == PaymentRespose.ResultType.Rejected)
                {
                    await bus.Publish(new PaymentRejected
                    {
                        ReferenceId = command.ReferenceId,
                        ConferenceId = command.ConferenceId,
                        BusinessCustomerId = command.BusinessCustomerId,
                        PaymentGatewayId = command.PaymentGatewayId,
                        Amount = command.Amount,
                        Reason = "No money."
                    });
                }
                else
                {
                    await bus.Publish(new PaymentAccepted
                    {
                        ReferenceId = command.ReferenceId,
                        ConferenceId = command.ConferenceId,
                        BusinessCustomerId = command.BusinessCustomerId,
                        PaymentGatewayId = command.PaymentGatewayId,
                        Amount = command.Amount
                    });
                }
            }
            catch (Exception e)
            {
                await bus.Publish(new PaymentRejected
                {
                    ReferenceId = command.ReferenceId,
                    ConferenceId = command.ConferenceId,
                    BusinessCustomerId = command.BusinessCustomerId,
                    PaymentGatewayId = command.PaymentGatewayId,
                    Amount = command.Amount,
                    Reason = "Can't complete the payment process (" + e.GetBaseException().Message + ")."
                });
            }
        }
    }
}