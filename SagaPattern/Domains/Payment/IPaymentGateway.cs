using System.Threading.Tasks;

namespace SagaPattern.Domains.Payment
{
    public interface IPaymentGateway
    {
        Task<PaymentRespose> PayAsync(PaymentRequest paymentRequest);
    }

    public class PaymentRequest
    {
        public string Customer { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentRespose
    {
        public enum ResultType
        {
            None,
            Accepted,
            Rejected
        }

        public string Customer { get; set; }
        public ResultType Result { get; set; }
    }
}