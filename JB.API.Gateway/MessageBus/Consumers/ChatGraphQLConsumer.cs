using JB.Gateway.DTOs.Chat;
using JB.Infrastructure.Messages;
using SlimMessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Gateway.MessageBus.Consumers
{
    public class ChatGraphQLConsumer : IConsumer<SubscriptionsMessageResponse>
    {
        public Task OnHandle(SubscriptionsMessageResponse message, string path)
        {
            throw new NotImplementedException();
        }
    }
}
   