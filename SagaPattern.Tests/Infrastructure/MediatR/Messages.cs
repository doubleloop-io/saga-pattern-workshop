using System;
using SagaPattern.Infrastructure;

namespace SagaPattern.Tests.Infrastructure.MediatR
{
    internal static class Messages
    {
        public class Message : IMessage
        {
            private readonly string value;

            public Message(string value)
            {
                this.value = value;
            }

            protected bool Equals(Message other)
            {
                return string.Equals(value, other.value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Message) obj);
            }

            public override int GetHashCode()
            {
                return (value != null ? value.GetHashCode() : 0);
            }
        }

        public class ChildMessage : Messages.Message
        {
            public ChildMessage(string value) : base(value)
            {
            }
        }

        public class UnrelatedMessage : IMessage
        {
            private readonly string value;

            public UnrelatedMessage(string value)
            {
                this.value = value;
            }

            protected bool Equals(UnrelatedMessage other)
            {
                return string.Equals(value, other.value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((UnrelatedMessage) obj);
            }

            public override int GetHashCode()
            {
                return (value != null ? value.GetHashCode() : 0);
            }
        }
    }
}