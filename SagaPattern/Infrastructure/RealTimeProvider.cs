using System;

namespace SagaPattern.Infrastructure
{
    public class RealTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}