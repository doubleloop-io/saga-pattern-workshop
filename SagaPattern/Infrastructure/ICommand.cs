using System;

namespace SagaPattern.Infrastructure
{
    public interface ICommand : IMessage
    {
        Guid Id { get; }
    }
}