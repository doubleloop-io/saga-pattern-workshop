using System;
using System.Threading.Tasks;
using SagaPattern.Infrastructure.MediatR;

namespace SagaPattern.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IBus Bus { get; private set; }
        public static IEventWaiter EventWaiter { get; private set; }

        public static void Bootstrap()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            var eventLogger = new EventLogger();

            Bus = MediatRBus.Create();
            Bus.Subscribe(eventLogger);
            EventWaiter = new EventWaiter(eventLogger);
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
        }

        static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Console.WriteLine(e.Exception.GetBaseException());
        }
    }
}
