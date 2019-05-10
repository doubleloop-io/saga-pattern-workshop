using System;
using System.IO;
using System.Threading.Tasks;
using SagaPattern.Domains.Inventory;
using SagaPattern.Domains.Payment;
using SagaPattern.Domains.Pricing;
using SagaPattern.Domains.Selling;
using SagaPattern.Infrastructure;
using static SagaPattern.Domains.Inventory.InventoryMessages;
using static SagaPattern.Domains.Payment.PaymentMessages;
using static SagaPattern.Domains.Pricing.PricingMessages;
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
            await SeatsAvailabilityCommands();
            await PaymentCommands();
            await PricingCommands();
            await OrderCommands();
        }

        private static async Task SeatsAvailabilityCommands()
        {
            var eventWaiter = InfrastructureModule.EventWaiter;
            var bus = InfrastructureModule.Bus;
            var availabilityId = Ids.New();

            await bus.Publish(new AddSeats
            {
                SeatsAvailabilityId = availabilityId,
                Quantity = 10
            });
            await eventWaiter.WaitForSingle<SeatsAdded>(
                x => x.SeatsAvailabilityId == availabilityId);

            await bus.Publish(new RemoveSeats
            {
                SeatsAvailabilityId = availabilityId,
                Quantity = 5
            });
            await eventWaiter.WaitForSingle<SeatsRemoved>(
                x => x.SeatsAvailabilityId == availabilityId);

            var reservationIdToBeCommitted = Ids.New();
            await bus.Publish(new MakeSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = reservationIdToBeCommitted,
                Quantity = 2
            });
            await eventWaiter.WaitForSingle<SeatsReservationAccepted>(
                x => x.ReservationId == reservationIdToBeCommitted);

            var reservationIdToBeCancelled = Ids.New();
            await bus.Publish(new MakeSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = reservationIdToBeCancelled,
                Quantity = 1
            });
            await eventWaiter.WaitForSingle<SeatsReservationAccepted>(
                x => x.ReservationId == reservationIdToBeCancelled);

            await bus.Publish(new MakeSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = Ids.New(),
                Quantity = 3
            });
            await eventWaiter.WaitForSingle<SeatsReservationRejected>(
                x => x.SeatsAvailabilityId == availabilityId);

            await bus.Publish(new CommitSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = reservationIdToBeCommitted
            });
            await eventWaiter.WaitForSingle<SeatsReservationCommitted>(
                x => x.ReservationId == reservationIdToBeCommitted);

            await bus.Publish(new CancelSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = reservationIdToBeCancelled
            });
            await eventWaiter.WaitForSingle<SeatsReservationCanceled>(
                x => x.ReservationId == reservationIdToBeCancelled);
        }

        private static async Task PaymentCommands()
        {
            var eventWaiter = InfrastructureModule.EventWaiter;
            var bus = InfrastructureModule.Bus;

            var succeedReferenceId = Ids.New();
            await bus.Publish(new MakePayment
            {
                ReferenceId = succeedReferenceId,
                Amount = 100,
                BusinessCustomerId = Ids.StarkIndustries,
                ConferenceId = Ids.LambdaWorld,
                PaymentGatewayId = Ids.PayPal
            });
            await eventWaiter.WaitForSingle<PaymentAccepted>(
                x => x.ReferenceId == succeedReferenceId);

            var rejectReferenceId = Ids.New();
            await bus.Publish(new MakePayment
            {
                ReferenceId = rejectReferenceId,
                Amount = 100,
                BusinessCustomerId = Ids.StarkIndustries,
                ConferenceId = Ids.LambdaWorld,
                PaymentGatewayId = Ids.Check
            });
            await eventWaiter.WaitForSingle<PaymentRejected>(
                x => x.ReferenceId == rejectReferenceId && x.Reason == "No money.");

            var failingReferenceId = Ids.New();
            await bus.Publish(new MakePayment
            {
                ReferenceId = failingReferenceId,
                Amount = 100,
                BusinessCustomerId = Ids.StarkIndustries,
                ConferenceId = Ids.LambdaWorld,
                PaymentGatewayId = Ids.Visa
            });
            await eventWaiter.WaitForSingle<PaymentRejected>(
                x => x.ReferenceId == failingReferenceId && x.Reason != "No money.");
        }

        private static async Task PricingCommands()
        {
            var eventWaiter = InfrastructureModule.EventWaiter;
            var bus = InfrastructureModule.Bus;

            var referenceId = Ids.New();
            await bus.Publish(new CalculatePrice
            {
                ReferenceId = referenceId,
                ConferenceId = Ids.LambdaWorld,
                Quantity = 10
            });
            await eventWaiter.WaitForSingle<PriceCalculated>(x => x.ReferenceId == referenceId);

        }

        private static async Task OrderCommands()
        {
            var eventWaiter = InfrastructureModule.EventWaiter;
            var bus = InfrastructureModule.Bus;
            var orderId = Ids.New();

            await bus.Publish(new PlaceOrder
            {
                OrderId = orderId,
                ConferenceId = Ids.LambdaWorld,
                Quantity = 10
            });
            await eventWaiter.WaitForSingle<OrderPlaced>(x => x.OrderId == orderId);

            await bus.Publish(new BookOrder
            {
                OrderId = orderId
            });
            await eventWaiter.WaitForSingle<OrderBooked>(x => x.OrderId == orderId);

            await bus.Publish(new PriceOrder
            {
                OrderId = orderId,
                Amount = 100M
            });
            await eventWaiter.WaitForSingle<OrderPriced>(x => x.OrderId == orderId);

            await bus.Publish(new SetCustomer
            {
                OrderId = orderId,
                CustomerId = Ids.JonSnow,
                BusinessCustomerId = Ids.StarkIndustries,
                PaymentGatewayId = Ids.PayPal,
            });
            await eventWaiter.WaitForSingle<CustomerSet>(x => x.OrderId == orderId);

            await bus.Publish(new ConfirmOrder
            {
                OrderId = orderId
            });
            await eventWaiter.WaitForSingle<OrderConfirmed>(x => x.OrderId == orderId);

            var orderIdToBeCancelled = Ids.New();

            await bus.Publish(new PlaceOrder
            {
                OrderId = orderIdToBeCancelled,
                ConferenceId = Ids.LambdaWorld,
                Quantity = 10
            });
            await eventWaiter.WaitForSingle<OrderPlaced>(x => x.OrderId == orderIdToBeCancelled);

            await bus.Publish(new CancelOrder
            {
                OrderId = orderIdToBeCancelled
            });
            await eventWaiter.WaitForSingle<OrderCanceled>(x => x.OrderId == orderIdToBeCancelled);
        }
    }
}
