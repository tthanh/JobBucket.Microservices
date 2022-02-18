using AutoMapper;
using JB.Infrastructure.DTOs.Subscriptions;
using JB.Notification.Models.Notification;
using SlimMessageBus;
using System;

namespace JB.Notification.GraphQL.Notification
{
    public class NotificationRedisPubSubObserver : IObserver<NotificationModel>
    {
        private readonly IMessageBus _messageBus;
        private readonly IMapper _mapper;

        public NotificationRedisPubSubObserver(IMessageBus messageBus,
            IMapper mapper)
        {
            _messageBus = messageBus;
            _mapper = mapper;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(NotificationModel value)
        {
            SubscriptionsNotificationResponse notiResponse = _mapper.Map<SubscriptionsNotificationResponse>(value);

            _messageBus.Publish(notiResponse);
        }
    }
}
