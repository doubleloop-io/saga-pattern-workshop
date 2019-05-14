using System.Collections.Generic;

namespace SagaPattern.Infrastructure
{
    public interface IEventLog
    {
        IEnumerable<IEvent> LoggedEvents();

        void Clear();
    }
}