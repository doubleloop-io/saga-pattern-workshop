namespace SagaPattern.Infrastructure
{
    public interface ICommandHandler<T> : IHandler<T> where T : ICommand
    {
    }
}