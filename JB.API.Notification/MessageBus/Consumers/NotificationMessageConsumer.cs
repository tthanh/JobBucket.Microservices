using JB.Infrastructure.Messages;
using SlimMessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.API.Notification.MessageBus.Consumers
{
    public class NotificationMessageConsumer : IConsumer<NotificationMessage>
    {
        public Task OnHandle(NotificationMessage message, string path)
        {
            throw new NotImplementedException();
        }
    }
}
