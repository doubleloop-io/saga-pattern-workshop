using MediatR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SagaPattern.Infrastructure.MediatR
{
    internal interface IHandlerWrapperResolver
    {
        object HandlersFor(Type messageType);
    }

    internal interface IHandlerWrapperRegistry
    {
        void Register<TMessage>(HandlerWrapper<TMessage> handlerWrapper) where TMessage : IMessage;
    }

    internal class HandlerWrapperDirectory : IHandlerWrapperResolver, IHandlerWrapperRegistry
    {
        readonly ConcurrentDictionary<Type, ConcurrentBag<object>> handlerWrappersByType = new ConcurrentDictionary<Type, ConcurrentBag<object>>();

        public void Register<TMessage>(HandlerWrapper<TMessage> handlerWrapper) where TMessage : IMessage
        {
            handlerWrappersByType.AddOrUpdate(typeof(TMessage),
                _ => new ConcurrentBag<object>(new[] {handlerWrapper}),
                (_, bag) =>
                {
                    bag.Add(handlerWrapper);
                    return bag;
                });
        }

        public object HandlersFor(Type messageType)
        {
            return TypedHandlersForMethod(messageType)
                .Invoke(this, new object[0]);
        }

        private MethodInfo TypedHandlersForMethod(Type messageType)
        {
            return GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Single(x => x.Name == nameof(TypedHandlersFor))
                .MakeGenericMethod(messageType);
        }

        private INotificationHandler<MessageWrapper<TMessage>>[] TypedHandlersFor<TMessage>() where TMessage : IMessage
        {
            return typeof(TMessage)
                .RelatedMessageTypes()
                .SelectMany(HandlersFor<TMessage>)
                .ToArray();
        }

        private IEnumerable<INotificationHandler<MessageWrapper<TMessage>>> HandlersFor<TMessage>(Type relatedType) where TMessage : IMessage
        {
            return (IEnumerable<INotificationHandler<MessageWrapper<TMessage>>>)
                AdaptHandlersMethod(typeof(TMessage), relatedType)
                    .Invoke(this, new object[0]);
        }

        private MethodInfo AdaptHandlersMethod(Type messageType, Type relatedType)
        {
            return GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Single(x => x.Name == nameof(AdaptHandlers))
                .MakeGenericMethod(messageType, relatedType);
        }

        private IEnumerable<INotificationHandler<MessageWrapper<TMessage>>> AdaptHandlers<TMessage, TRelated>()
            where TMessage : IMessage, TRelated
            where TRelated: IMessage
        {
            return handlerWrappersByType.GetOrAdd(typeof(TRelated), _ => new ConcurrentBag<object>())
                .Cast<HandlerWrapper<TRelated>>()
                .Select(Adapt<TMessage, TRelated>);
        }

        private static INotificationHandler<MessageWrapper<TMessage>> Adapt<TMessage, TRelated>(HandlerWrapper<TRelated> h) 
            where TMessage : IMessage, TRelated where TRelated: IMessage
        {
            return typeof(TMessage) == typeof(TRelated)
                ? (INotificationHandler<MessageWrapper<TMessage>>) (h as HandlerWrapper<TMessage>)
                : new HandlerWrapperAdapter<TMessage, TRelated>(h);
        }
    }
}