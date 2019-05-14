using System;

namespace SagaPattern.Infrastructure
{
    public interface IScheduler
    {
        void Schedule(TimeSpan after, Action<IScheduler, object> callback, object state);
    }
}