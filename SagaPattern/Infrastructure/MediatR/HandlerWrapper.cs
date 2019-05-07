using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SagaPattern.Infrastructure.MediatR
{
    internal class HandlerWrapper<T> : INotificationHandler<MessageWrapper<T>>
        where T: IMessage
    {
        private readonly IHandler<T> handler;

        public HandlerWrapper(IHandler<T> handler)
        {
            this.handler = handler;
        }

        public Task Handle(MessageWrapper<T> notification, CancellationToken cancellationToken)
        {
            return handler.Handle(notification.Message);
        }
    }
}