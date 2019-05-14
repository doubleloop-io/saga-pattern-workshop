using System.Collections.Generic;
using System.Threading.Tasks;

namespace SagaPattern.Infrastructure
{
    public class EventLogger : IEventHandler<IEvent>, IEventLog
    {
        readonly object lockHandle = new object();
        readonly List<IEvent> journal = new List<IEvent>();

        public Task Handle(IEvent message)
        {
            lock (lockHandle)
            {
                journal.Add(message);
            }
            return Task.CompletedTask;
        }

        public IEnumerable<IEvent> LoggedEvents()
        {
            lock (lockHandle)
            {
                return journal.ToArray();
            }
        }

        public void Clear()
        {
            lock (lockHandle)
            {
                journal.Clear();
            }
        }
    }
}