using System;

namespace SagaPattern.Infrastructure
{
    public interface IEvent : IMessage
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
    }
}