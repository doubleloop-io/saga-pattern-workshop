using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SagaPattern.Infrastructure;

namespace SagaPattern.Tests.Infrastructure.MediatR
{
    internal static class Handlers
    {
        public class Handler<T> : IHandler<T> where T : IMessage
        {
            public Handler()
            {
                ReceivedMessages = new List<IMessage>();
            }

            public readonly SemaphoreSlim MessageReceived = new SemaphoreSlim(0);
            public List<IMessage> ReceivedMessages { get; }

            public virtual async Task Handle(T message)
            {
                ReceivedMessages.Add(message);
                MessageReceived.Release();
                await Task.CompletedTask;
            }
        }

        public class ThrowingHandler<T> : Handler<T> where T : IMessage
        {
            public override Task Handle(T message)
            {
                var ignore = base.Handle(message);
                throw new InvalidOperationException("Throwing exception");
            }
        }
    }
}