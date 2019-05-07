using FluentAssertions;
using SagaPattern.Infrastructure;
using SagaPattern.Infrastructure.MediatR;
using System;
using System.Threading.Tasks;
using Xunit;
using static SagaPattern.Tests.Infrastructure.MediatR.Handlers;
using static SagaPattern.Tests.Infrastructure.MediatR.Messages;

namespace SagaPattern.Tests.Infrastructure.MediatR
{
    public class MediatRBusTests
    {
        private IBus bus;

        public MediatRBusTests()
        {
            bus = MediatRBus.Create();
        }

        [Fact]
        public async Task route_published_message_to_subscribed_handler()
        {
            var handler = new Handler<Message>();
            bus.Subscribe(handler);

            PublishWithoutWaiting(new Message("test value"));

            await HandlerReceivedMessage(handler, new Message("test value"));
        }

        [Fact]
        public async Task route_all_published_message_to_subscribed_handler()
        {
            var handler = new Handler<Message>();
            bus.Subscribe(handler);

            PublishWithoutWaiting(new Message("test value"));
            PublishWithoutWaiting(new Message("other test value"));

            await TryWaitMessageReceived(handler, 2);
            handler.ReceivedMessages.Should().BeEquivalentTo(new Message("test value"), new Message("other test value"));
        }

        [Fact]
        public async Task not_route_published_message_to_handler_of_other_message()
        {
            var handler = new Handler<Message>();
            var unrelatedHandler = new Handler<UnrelatedMessage>();
            bus.Subscribe(handler);
            bus.Subscribe(unrelatedHandler);

            PublishWithoutWaiting(new Message("test value"));

            await handler.MessageReceived.WaitAsync(TimeSpan.FromSeconds(1));
            unrelatedHandler.ReceivedMessages.Should().BeEmpty();
        }

        [Fact]
        public async Task route_published_message_to_handler_of_base_type()
        {
            var handler = new Handler<Message>();
            var handlerInterface = new Handler<IMessage>();
            var handlerChild = new Handler<ChildMessage>();
            bus.Subscribe(handler);
            bus.Subscribe(handlerChild);
            bus.Subscribe(handlerInterface);

            PublishWithoutWaiting(new ChildMessage("test value"));
            PublishWithoutWaiting(new UnrelatedMessage("test value B"));

            await HandlerReceivedMessage(handlerChild, new ChildMessage("test value"));
            await HandlerReceivedMessage(handler, new ChildMessage("test value"));
            await TryWaitMessageReceived(handlerInterface);
            handlerInterface.ReceivedMessages.Should().BeEquivalentTo(
                new ChildMessage("test value"), 
                new UnrelatedMessage("test value B"));
        }

        [Fact]
        public async Task route_published_message_to_all_subscribed_handlers()
        {
            var handler = new Handler<Message>();
            var otherHandler = new Handler<Message>();
            bus.Subscribe(handler);
            bus.Subscribe(otherHandler);

            PublishWithoutWaiting(new Message("test value"));

            await HandlerReceivedMessage(otherHandler, new Message("test value"));
            await HandlerReceivedMessage(handler, new Message("test value"));
        }

        [Fact]
        public async Task route_published_message_even_on_exception_thrown()
        {
            var handler = new Handler<Message>();
            bus.Subscribe(new ThrowingHandler<Message>());
            bus.Subscribe(handler);
            bus.Subscribe(new ThrowingHandler<Message>());

            PublishWithoutWaiting(new Message("test value"));

            await HandlerReceivedMessage(handler, new Message("test value"));
        }

        private void PublishWithoutWaiting<T>(T message) where T : IMessage
        {
            bus.Publish(message);
        }

        private static async Task HandlerReceivedMessage<T>(Handler<T> handler, T expectedMessage)
            where T : IMessage
        {
            await TryWaitMessageReceived(handler);
            handler.ReceivedMessages.Should().BeEquivalentTo(expectedMessage);
        }

        private static async Task TryWaitMessageReceived<T>(Handler<T> handler, int howMany = 1) where T : IMessage
        {
            for (var i = 0; i < howMany; i++)
            {
                await handler.MessageReceived.WaitAsync(TimeSpan.FromSeconds(1));
            }
        }
    }
}