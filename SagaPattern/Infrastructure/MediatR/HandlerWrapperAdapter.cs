using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SagaPattern.Infrastructure.MediatR
{
    internal class HandlerWrapperAdapter<T, TRelated> : INotificationHandler<MessageWrapper<T>> 
        where T : IMessage, TRelated 
        where TRelated : IMessage
    {
        private readonly HandlerWrapper<TRelated> handler;

        public HandlerWrapperAdapter(HandlerWrapper<TRelated> wrapper)
        {
            handler = wrapper;
        }

        public Task Handle(MessageWrapper<T> notification, CancellationToken cancellationToken)
        {
            return handler.Handle(new MessageWrapper<TRelated>(notification.Message), cancellationToken);
        }
    }
}