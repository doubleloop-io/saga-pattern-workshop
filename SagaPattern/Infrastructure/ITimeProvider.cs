using System;

namespace SagaPattern.Infrastructure
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}