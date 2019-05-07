using System.Threading.Tasks;

namespace SagaPattern.Infrastructure
{
    public interface IEventHandler<T> : IHandler<T> where T : IEvent
    {
    }
}