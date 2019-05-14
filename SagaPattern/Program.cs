using SagaPattern.Domains.Inventory;
using SagaPattern.Domains.Payment;
using SagaPattern.Domains.Pricing;
using SagaPattern.Domains.Selling;
using SagaPattern.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;
using static SagaPattern.Domains.Inventory.InventoryMessages;
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
            await Exercise1();
        }

        private static async Task Exercise1()
        {
            var eventWaiter = InfrastructureModule.EventWaiter;
            var bus = InfrastructureModule.Bus;
            var orderId = Ids.New();

            await bus.Publish(new AddSeats
            {
                SeatsAvailabilityId = Ids.LambdaWorld,
                Quantity = 10
            });
            await eventWaiter.WaitForSingle<SeatsAdded>(x => x.SeatsAvailabilityId == Ids.LambdaWorld);

            await bus.Publish(new PlaceOrder
            {
                OrderId = orderId,
                ConferenceId = Ids.LambdaWorld,
                Quantity = 10
            });
            await eventWaiter.WaitForSingle<SeatsReservationAccepted>(
                x => x.SeatsAvailabilityId == Ids.LambdaWorld && x.ReservationId == orderId && x.Quantity == 10);

            Console.WriteLine("Yeah, you did it!!! ;-)");
        }
    }
}
