using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using JB.Infrastructure.Messages;
using JB.Notification.Models.Notification;
using JB.Notification.Services;
using SlimMessageBus;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.GraphQL.Notification
{
    public class NotificationRedisPubSubObserver : IObserver<NotificationModel>
    {
        private readonly IJwtService _jwtService;
        private readonly IMessageBus _messageBus;

        public NotificationRedisPubSubObserver(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(NotificationModel value)
        {
            _messageBus.Publish(new { Id = 1 },"graphql_notification");
        }
    }
}
