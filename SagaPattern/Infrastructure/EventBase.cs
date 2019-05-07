using System;

namespace SagaPattern.Infrastructure
{
    public abstract class EventBase : IEvent
    {
        protected EventBase()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public DateTime OccurredOn { get; }
    }
}