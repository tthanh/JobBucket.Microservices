using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using JB.Notification.Models.Notification;
using JB.Notification.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.GraphQL.Notification
{
    public class NotificationRedisPubSubObserver : IObserver<NotificationModel>
    {
        private readonly ITopicEventSender _topicEventSender;
        private readonly ITopicEventReceiver _topicEventReceiver;
        private readonly IJwtService _jwtService;

        public NotificationRedisPubSubObserver(
            ITopicEventSender topicEventSender)
        {
            _topicEventSender = topicEventSender;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(NotificationModel value)
        {
            Task.Run(() => _topicEventSender.SendAsync($"notification_{value.ReceiverId}", value));
        }
    }
}
