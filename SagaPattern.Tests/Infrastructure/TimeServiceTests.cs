using System;
using System.Threading.Tasks;
using FluentAssertions;
using SagaPattern.Infrastructure;
using SagaPattern.Infrastructure.MediatR;
using SagaPattern.Infrastructure.Scheduler;
using SagaPattern.Tests.Infrastructure.MediatR;
using Xunit;
namespace SagaPattern.Tests.Infrastructure
{
    public class TimeServiceTests : IHandler<Messages.Message>
    {
        private Messages.Message receivedMessage;

        [Fact]
        public async Task schedule_event()
        {
            var bus = MediatRBus.Create();
            var scheduler = new ThreadBasedScheduler(new RealTimeProvider());
            var timeService = new TimerService(scheduler, bus);
            bus.Subscribe(timeService);
            bus.Subscribe(this);

            await bus.Publish(new TimerMessages.Schedule
            {
                TriggerAfter = TimeSpan.FromMilliseconds(200),
                Message = new Messages.Message("Test value")
            });
            await Task.Delay(500);

            receivedMessage.Should().NotBeNull();
        }

        Task IHandler<Messages.Message>.Handle(Messages.Message message)
        {
            receivedMessage = message;
            return Task.CompletedTask;
        }
    }
}