using System.Threading.Tasks;

namespace SagaPattern.Infrastructure
{
    public interface IHandler<T>
        where T : IMessage
    {
        Task Handle(T message);
    }
}