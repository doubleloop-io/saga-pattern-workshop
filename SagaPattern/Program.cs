using System;
using System.Threading.Tasks;
using SagaPattern.Infrastructure;

namespace SagaPattern
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            InfrastructureModule.Bootstrap();

            Task.Run(async () =>
            {
                await InfrastructureModule.Bus.Publish(new TestEvent { Value = "The wrong one" });
                Console.WriteLine("Wrong one sent");
                await Task.Delay(1000);
                await InfrastructureModule.Bus.Publish(new TestEvent {Value = "The right one"});
                Console.WriteLine("Right one sent");
            });
            var @event = InfrastructureModule.EventWaiter.WaitForSingle<TestEvent>(x => x.Value == "The right one", TimeSpan.FromSeconds(2)).Result;

            Console.WriteLine(@event == null ? "Something is broken" : "It works");
            Console.ReadLine();
        }

        class TestEvent : EventBase
        {
            public string Value { get; set; }
        }
    }
}
