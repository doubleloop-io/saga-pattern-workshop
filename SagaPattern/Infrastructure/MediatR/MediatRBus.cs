using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SagaPattern.Infrastructure.MediatR
{
    public class MediatRBus : IBus
    {
        private readonly IMediator mediator;
        private readonly IHandlerWrapperRegistry registry;

        private MediatRBus(IHandlerWrapperRegistry registry, IMediator mediator)
        {
            this.mediator = mediator;
            this.registry = registry;
        }

        public void Subscribe<T>(IHandler<T> handler) where T : IMessage
        {
            registry.Register(new HandlerWrapper<T>(handler));
        }

        public Task Publish(IMessage message)
        {
            return (Task)TypedPublishMethod(message.GetType())
                .Invoke(this, new object[] {message});
        }

        private MethodInfo TypedPublishMethod(Type messageType)
        {
            return GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Single(x => x.Name == nameof(Publish) && x.IsGenericMethodDefinition)
                .MakeGenericMethod(messageType);
        }

        public async Task Publish<T>(T message) where T : IMessage
        {
            await mediator.Publish(new MessageWrapper<T>(message));
        }

        public static IBus Create()
        {
            var directory = new HandlerWrapperDirectory();
            var mediator = new ThreadedMediator(CreateServiceFactory(directory));

            return new MediatRBus(directory, mediator);
        }

        private static ServiceFactory CreateServiceFactory(IHandlerWrapperResolver directory)
        {
            return type =>
            {
                var enumerableType = EnumerableTypeOrThrow(type);
                var notificationHandlerType = enumerableType.GetGenericArguments()[0];
                var handlerWrapperType = notificationHandlerType.GetGenericArguments()[0];
                var messageType = handlerWrapperType.GetGenericArguments()[0];

                return directory.HandlersFor(messageType);
            };
        }

        private static Type EnumerableTypeOrThrow(Type type)
        {
            var enumerableType = type
                .GetInterfaces()
                .Concat(new[] {type})
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerableType == null)
            {
                throw new InvalidOperationException("This MediatR instance supports only publish-subscribe");
            }
            return enumerableType;
        }
    }
}