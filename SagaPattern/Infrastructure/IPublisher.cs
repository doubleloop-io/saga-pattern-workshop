using System.Threading.Tasks;

namespace SagaPattern.Infrastructure
{
    public interface IPublisher
    {
        Task Publish(IMessage message);
        Task Publish<T>(T message) where T : IMessage;
    }
}