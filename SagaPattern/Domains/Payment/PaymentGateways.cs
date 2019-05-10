using System;
using System.Threading.Tasks;

namespace SagaPattern.Domains.Payment
{
    public class PaymentGatewayAlwaysAccept : IPaymentGateway
    {
        readonly int millisecondsDelay;

        public PaymentGatewayAlwaysAccept(int delay)
        {
            millisecondsDelay = delay;
        }

        public async Task<PaymentRespose> PayAsync(PaymentRequest paymentRequest)
        {
            await Task.Delay(millisecondsDelay);
            return new PaymentRespose
            {
                Customer = paymentRequest.Customer,
                Result = PaymentRespose.ResultType.Accepted
            };
        }
    }

    public class PaymentGatewayAlwaysReject : IPaymentGateway
    {
        readonly int millisecondsDelay;

        public PaymentGatewayAlwaysReject(int delay)
        {
            millisecondsDelay = delay;
        }

        public async Task<PaymentRespose> PayAsync(PaymentRequest paymentRequest)
        {
            await Task.Delay(millisecondsDelay);
            return new PaymentRespose
            {
                Customer = paymentRequest.Customer,
                Result = PaymentRespose.ResultType.Rejected
            };
        }
    }

    public class PaymentGatewayAlwaysFail : IPaymentGateway
    {
        readonly int millisecondsDelay;

        public PaymentGatewayAlwaysFail(int delay)
        {
            millisecondsDelay = delay;
        }

        public async Task<PaymentRespose> PayAsync(PaymentRequest paymentRequest)
        {
            await Task.Delay(millisecondsDelay);
            throw new InvalidOperationException("Unable to connect");
        }
    }
}