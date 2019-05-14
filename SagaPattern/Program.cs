using System;
using System.IO;
using System.Threading.Tasks;
using SagaPattern.Domains.Inventory;
using SagaPattern.Domains.Payment;
using SagaPattern.Domains.Pricing;
using SagaPattern.Domains.Process;
using SagaPattern.Domains.Selling;
using SagaPattern.Infrastructure;
using static SagaPattern.Domains.Inventory.InventoryMessages;
using static SagaPattern.Domains.Payment.PaymentMessages;
using static SagaPattern.Domains.Selling.SellingMessages;

namespace SagaPattern
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Bootstrap();
            CleanStores();

            MainAsync().Wait();

            Console.WriteLine("Press <ENTER> to exit");
            Console.ReadLine();
        }

        private static void Bootstrap()
        {
            InfrastructureModule.Bootstrap();
            InventoryModule.Bootstrap(InfrastructureModule.Bus);
            PaymentModule.Bootstrap(InfrastructureModule.Bus);
            PricingModule.Bootstrap(InfrastructureModule.Bus);
            SellingModule.Bootstrap(InfrastructureModule.Bus);
            ProcessModule.Bootstrap(InfrastructureModule.Bus);
        }

        static void CleanStores()
        {
            var files = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.data");
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private static async Task MainAsync()
        {
            await Exercise2();
            await Exercise3();
        }

        private static async Task Exercise2()
        {
            var eventWaiter = InfrastructureModule.EventWaiter;
            var bus = InfrastructureModule.Bus;

            await bus.Publish(new AddSeats
            {
                SeatsAvailabilityId = Ids.LambdaWorld,
                Quantity = 10
            });
            await eventWaiter.WaitForSingle<SeatsAdded>(
                x => x.SeatsAvailabilityId == Ids.LambdaWorld);

            var orderId = Ids.New();

            await bus.Publish(new PlaceOrder
            {
                OrderId = orderId,
                ConferenceId = Ids.LambdaWorld,
                Quantity = 2
            });
            await eventWaiter.WaitForSingle<SeatsReservationAccepted>(
                x => x.ReservationId == orderId && x.Quantity == 2);
            await eventWaiter.WaitForSingle<OrderPriced>(x => x.OrderId == orderId && x.Amount == 40);

            await bus.Publish(new SetCustomer
            {
                OrderId = orderId,
                CustomerId = Ids.JonSnow,
                BusinessCustomerId = Ids.StarkIndustries,
                PaymentGatewayId = Ids.PayPal
            });
            await eventWaiter.WaitForSingle<PaymentAccepted>(x => x.ReferenceId == orderId
                                                                  && x.BusinessCustomerId == Ids.StarkIndustries
                                                                  && x.PaymentGatewayId == Ids.PayPal
                                                                  && x.ConferenceId == Ids.LambdaWorld
                                                                  && x.Amount == 40);
            await eventWaiter.WaitForSingle<SeatsReservationCommitted>(x => x.ReservationId == orderId);
            await eventWaiter.WaitForSingle<OrderConfirmed>(x => x.OrderId == orderId);

            Console.WriteLine("Yeah, you did it!!! ;-)");
        }

        private static async Task Exercise3()
        {
            var eventWaiter = InfrastructureModule.EventWaiter;
            var bus = InfrastructureModule.Bus;

            await bus.Publish(new AddSeats
            {
                SeatsAvailabilityId = Ids.WorkingSoftwareConference,
                Quantity = 10
            });
            await eventWaiter.WaitForSingle<SeatsAdded>(x => x.SeatsAvailabilityId == Ids.WorkingSoftwareConference);

            var orderId = Ids.New();

            await bus.Publish(new PlaceOrder
            {
                OrderId = orderId,
                ConferenceId = Ids.WorkingSoftwareConference,
                Quantity = 3
            });
            await eventWaiter.WaitForSingle<OrderPriced>(x => x.OrderId == orderId);

            await bus.Publish(new SetCustomer
            {
                OrderId = orderId,
                CustomerId = Ids.JonSnow,
                BusinessCustomerId = Ids.StarkIndustries,
                PaymentGatewayId = Ids.Check
            });
            await eventWaiter.WaitForSingle<PaymentRejected>(x => x.ReferenceId == orderId);
            await eventWaiter.WaitForSingle<SeatsReservationCanceled>(x => x.ReservationId == orderId);
            await eventWaiter.WaitForSingle<OrderCanceled>(x => x.OrderId == orderId && x.Reason == "No money.");

            Console.WriteLine("Yeah, you did it!!! ;-)");
        }
    }
}
