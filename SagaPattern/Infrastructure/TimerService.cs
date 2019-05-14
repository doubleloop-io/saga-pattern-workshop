using System.Threading.Tasks;

namespace SagaPattern.Infrastructure
{
    public class TimerService : ICommandHandler<TimerMessages.Schedule>
    {
        private readonly IScheduler scheduler;
        private readonly IPublisher channel;

        public TimerService(IScheduler scheduler, IPublisher channel)
        {
            this.scheduler = scheduler;
            this.channel = channel;
        }

        public Task Handle(TimerMessages.Schedule message)
        {
            scheduler.Schedule(message.TriggerAfter, OnTimerCallback, message);
            return Task.CompletedTask;
        }

        private void OnTimerCallback(IScheduler s, object state)
        {
            var schedule = (TimerMessages.Schedule)state;
            channel.Publish(schedule.Message);
        }
    }
}