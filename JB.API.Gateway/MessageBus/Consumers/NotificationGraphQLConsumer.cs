using HotChocolate.Subscriptions;
using JB.Gateway.DTOs.Notification;
using JB.Infrastructure.Messages;
using SlimMessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Gateway.MessageBus.Consumers
{
    public class NotificationGraphQLConsumer : IConsumer<SubscriptionsNotificationResponse>
    {
        private readonly ITopicEventSender _topicEventSender;
        public NotificationGraphQLConsumer(ITopicEventSender topicEventSender)
        {
            _topicEventSender = topicEventSender;
        }
        public async Task OnHandle(SubscriptionsNotificationResponse message, string path)
        {
            await _topicEventSender.SendAsync("notification_40", message);
        }
    }
}
   