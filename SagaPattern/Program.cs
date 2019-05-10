using System;
using System.IO;
using System.Threading.Tasks;
using SagaPattern.Domains.Inventory;
using SagaPattern.Domains.Payment;
using SagaPattern.Infrastructure;
using static SagaPattern.Domains.Inventory.InventoryMessages;
using static SagaPattern.Domains.Payment.PaymentMessages;

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
            Console.WriteLine("Seats added");

            await bus.Publish(new RemoveSeats
            {
                SeatsAvailabilityId = availabilityId,
                Quantity = 5
            });
            await eventWaiter.WaitForSingle<SeatsRemoved>(
                x => x.SeatsAvailabilityId == availabilityId);
            Console.WriteLine("Seats removed");

            var reservationIdToBeCommitted = Ids.New();
            await bus.Publish(new MakeSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = reservationIdToBeCommitted,
                Quantity = 2
            });
            await eventWaiter.WaitForSingle<SeatsReservationAccepted>(
                x => x.ReservationId == reservationIdToBeCommitted);
            Console.WriteLine("Seats reserved");

            var reservationIdToBeCancelled = Ids.New();
            await bus.Publish(new MakeSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = reservationIdToBeCancelled,
                Quantity = 1
            });
            await eventWaiter.WaitForSingle<SeatsReservationAccepted>(
                x => x.ReservationId == reservationIdToBeCancelled);
            Console.WriteLine("Other seats reserved");

            await bus.Publish(new MakeSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = Ids.New(),
                Quantity = 3
            });
            await eventWaiter.WaitForSingle<SeatsReservationRejected>(
                x => x.SeatsAvailabilityId == availabilityId);
            Console.WriteLine("Reservation rejected");

            await bus.Publish(new CommitSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = reservationIdToBeCommitted
            });
            await eventWaiter.WaitForSingle<SeatsReservationCommitted>(
                x => x.ReservationId == reservationIdToBeCommitted);
            Console.WriteLine("Reservation committed");

            await bus.Publish(new CancelSeatsReservation
            {
                SeatsAvailabilityId = availabilityId,
                ReservationId = reservationIdToBeCancelled
            });
            await eventWaiter.WaitForSingle<SeatsReservationCanceled>(
                x => x.ReservationId == reservationIdToBeCancelled);
            Console.WriteLine("Reservation cancelled");
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
                BusinessCustomerId = Ids.DoubleLoop,
                ConferenceId = Ids.LambdaWorld,
                PaymentGatewayId = Ids.PayPal
            });
            await eventWaiter.WaitForSingle<PaymentAccepted>(
                x => x.ReferenceId == succeedReferenceId);
            Console.WriteLine("Payment succeeded");

            var rejectReferenceId = Ids.New();
            await bus.Publish(new MakePayment
            {
                ReferenceId = rejectReferenceId,
                Amount = 100,
                BusinessCustomerId = Ids.DoubleLoop,
                ConferenceId = Ids.LambdaWorld,
                PaymentGatewayId = Ids.Check
            });
            await eventWaiter.WaitForSingle<PaymentRejected>(
                x => x.ReferenceId == rejectReferenceId && x.Reason == "No money.");
            Console.WriteLine("Payment failed");

            var failingReferenceId = Ids.New();
            await bus.Publish(new MakePayment
            {
                ReferenceId = failingReferenceId,
                Amount = 100,
                BusinessCustomerId = Ids.DoubleLoop,
                ConferenceId = Ids.LambdaWorld,
                PaymentGatewayId = Ids.Visa
            });
            await eventWaiter.WaitForSingle<PaymentRejected>(
                x => x.ReferenceId == failingReferenceId && x.Reason != "No money.");
            Console.WriteLine("Payment rejected");
        }
    }
}
