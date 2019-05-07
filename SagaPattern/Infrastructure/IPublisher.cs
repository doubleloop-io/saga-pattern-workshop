using System.Threading.Tasks;

namespace SagaPattern.Infrastructure
{
    public interface IPublisher
    {
        Task Publish<T>(T message) where T : IMessage;
    }
}