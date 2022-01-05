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
    [ExtendObjectType(OperationTypeNames.Subscription)]
    public class NotificationSubscriptions : IObserver<NotificationModel>
    {
        private readonly ITopicEventSender _topicEventSender;
        private readonly ITopicEventReceiver _topicEventReceiver;
        private readonly IJwtService _jwtService;

        public NotificationSubscriptions(
            ITopicEventSender topicEventSender,
            ITopicEventReceiver topicEventReceiver,
            IJwtService jwtService)
        {
            _topicEventSender = topicEventSender;
            _topicEventReceiver = topicEventReceiver;
            _jwtService = jwtService;
        }

        [SubscribeAndResolve]
        [Topic("notification")]
        public ValueTask<ISourceStream<NotificationModel>> Notification(string token)
        {
            if (_jwtService.ValidateToken(token, out var jwtToken))
            {
                if (int.TryParse(jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value, out int userId))
                {
                    var topic = $"notification_{userId}";

                    return _topicEventReceiver.SubscribeAsync<string, NotificationModel>(topic);
                }
            }

            return new ValueTask<ISourceStream<NotificationModel>>();
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
