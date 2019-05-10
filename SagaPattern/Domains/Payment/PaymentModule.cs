using System;
using System.Collections.Generic;
using SagaPattern.Infrastructure;

namespace SagaPattern.Domains.Payment
{
    public static class PaymentModule
    {
        public static void Bootstrap(IBus bus)
        {
            var registry = new PaymentGatewaysRegistry(new Dictionary<Guid, IPaymentGateway>
            {
                {Ids.PayPal, new PaymentGatewayAlwaysAccept(3000)},
                {Ids.Check, new PaymentGatewayAlwaysReject(2000)},
                {Ids.Visa, new PaymentGatewayAlwaysFail(1000)}
            });
            var paymentHandler = new PaymentHandler(registry, bus);

            bus.Subscribe(paymentHandler);
        }
    }
}