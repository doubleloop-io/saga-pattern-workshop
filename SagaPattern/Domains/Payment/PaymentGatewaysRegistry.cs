using System;
using System.Collections.Generic;

namespace SagaPattern.Domains.Payment
{
    public class PaymentGatewaysRegistry
    {
        readonly IDictionary<Guid, IPaymentGateway> paymentGateways;

        public PaymentGatewaysRegistry(IDictionary<Guid, IPaymentGateway> paymentGateways)
        {
            this.paymentGateways = paymentGateways;
        }

        public IPaymentGateway For(Guid paymentGatewayId)
        {
            if (!paymentGateways.ContainsKey(paymentGatewayId))
                throw new InvalidOperationException("Missing payment gateway: " + paymentGatewayId);

            return paymentGateways[paymentGatewayId];
        }
    }
}