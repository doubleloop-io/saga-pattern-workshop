using System;

namespace SagaPattern.Infrastructure
{
    public static class TimerMessages
    {
        public class Schedule : CommandBase
        {
            public TimeSpan TriggerAfter { get; set; }
            public IMessage Message { get; set; }
        }

    }
}