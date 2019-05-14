using System;
using System.Threading.Tasks;
using SagaPattern.Infrastructure.MediatR;
using SagaPattern.Infrastructure.Scheduler;

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
            Bus.Subscribe(new MessageLogger());
            EventWaiter = new EventWaiter(eventLogger);

            var scheduler = new ThreadBasedScheduler(new RealTimeProvider());
            var timeService = new TimerService(scheduler, Bus);

            Bus.Subscribe(timeService);
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
