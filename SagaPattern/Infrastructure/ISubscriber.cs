namespace SagaPattern.Infrastructure
{
    public interface ISubscriber
    {
        void Subscribe<T>(IHandler<T> handler) where T : IMessage;
    }
}