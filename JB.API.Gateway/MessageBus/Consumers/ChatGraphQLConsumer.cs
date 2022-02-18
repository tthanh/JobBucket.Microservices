using HotChocolate.Subscriptions;
using JB.Infrastructure.DTOs.Subscriptions;
using SlimMessageBus;
using System;
using System.Threading.Tasks;

namespace JB.Gateway.MessageBus.Consumers
{
    public class ChatGraphQLConsumer : IConsumer<SubscriptionsMessageResponse>
    {
        private readonly ITopicEventSender _topicEventSender;
        public ChatGraphQLConsumer(ITopicEventSender topicEventSender)
        {
            _topicEventSender = topicEventSender;
        }

        public async Task OnHandle(SubscriptionsMessageResponse message, string path)
        {
            await _topicEventSender.SendAsync($"chat_{message.ReceiverId}", message);
        }
    }
}