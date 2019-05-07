using System;
using System.Threading.Tasks;

namespace SagaPattern.Infrastructure
{
    public interface IEventWaiter
    {

        Task<TEvent> WaitForSingle<TEvent>(Func<TEvent, bool> filter, TimeSpan timeout)
            where TEvent : IEvent;
    }
}