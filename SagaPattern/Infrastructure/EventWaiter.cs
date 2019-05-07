using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SagaPattern.Infrastructure
{
    public class EventWaiter : IEventWaiter
    {
        private readonly IEventLog eventLog;

        public EventWaiter(IEventLog eventLog)
        {
            this.eventLog = eventLog;
        }

        public async Task<TEvent> WaitForSingle<TEvent>(Func<TEvent, bool> filter, TimeSpan timeout)
            where TEvent: IEvent
        {
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < timeout)
            {
                var found = eventLog.LoggedEvents()
                    .OfType<TEvent>()
                    .SingleOrDefault(filter);

                if (found != null)
                {
                    return found;
                }
                await Task.Delay(100);
            }
            return default(TEvent);
        }
    }
}