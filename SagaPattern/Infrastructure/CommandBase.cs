using System;

namespace SagaPattern.Infrastructure
{
    public class CommandBase : ICommand
    {
        public CommandBase()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
    }
}