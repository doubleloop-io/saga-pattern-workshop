using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace SagaPattern.Infrastructure.MediatR
{
    internal class ThreadedMediator : Mediator
    {
        public ThreadedMediator(ServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        protected override Task PublishCore(IEnumerable<Func<Task>> allHandlers)
        {
            allHandlers.AsParallel().Select(handler => handler()).ToList();
            return Task.CompletedTask;
        }
    }
}