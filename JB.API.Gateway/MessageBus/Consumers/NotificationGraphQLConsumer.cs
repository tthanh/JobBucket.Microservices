using HotChocolate.Subscriptions;
using JB.Infrastructure.DTOs.Subscriptions;
using SlimMessageBus;
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
            await _topicEventSender.SendAsync($"notification_{message.ReceiverId}", message);
        }
    }
}