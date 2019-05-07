using FluentAssertions;
using SagaPattern.Infrastructure;
using Xunit;
using static SagaPattern.Tests.Infrastructure.MediatR.Messages;

namespace SagaPattern.Tests.Infrastructure
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void itself_is_a_related_message_type()
        {
            typeof(Message).RelatedMessageTypes().Should().Contain(typeof(Message));
        }

        [Fact]
        public void base_class_is_a_related_message_type()
        {
            typeof(ChildMessage).RelatedMessageTypes().Should().Contain(typeof(Message));
        }

        [Fact]
        public void IMessage_is_a_related_message_type()
        {
            typeof(Message).RelatedMessageTypes().Should().Contain(typeof(IMessage));
        }
    }
}