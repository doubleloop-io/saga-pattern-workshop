using MediatR;

namespace SagaPattern.Infrastructure.MediatR
{
    internal class MessageWrapper<T> : INotification where T: IMessage
    {
        public T Message { get; }

        public MessageWrapper(T message)
        {
            Message = message;
        }
    }
}