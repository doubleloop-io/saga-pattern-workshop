using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace SagaPattern.Infrastructure
{
    public class MessageLogger : IHandler<IMessage>
    {
        static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public Task Handle(IMessage message)
        {
            Log.Info(MessageName(message));
            return Task.CompletedTask;
        }

        private static string MessageName(IMessage message)
        {
            return message.GetType().FullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
        }
    }
}