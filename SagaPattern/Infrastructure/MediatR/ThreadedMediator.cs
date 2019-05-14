using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;

namespace SagaPattern.Infrastructure.MediatR
{
    internal class ThreadedMediator : Mediator
    {
        static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public ThreadedMediator(ServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        protected override Task PublishCore(IEnumerable<Func<Task>> allHandlers)
        {
            allHandlers.AsParallel().Select(TryHandle).ToList();
            return Task.CompletedTask;
        }

        private async Task TryHandle(Func<Task> handler)
        {
            try
            {
                await handler();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}