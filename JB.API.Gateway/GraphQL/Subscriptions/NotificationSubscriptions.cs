using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using JB.Gateway.Services;
using JB.Infrastructure.DTOs.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Gateway.GraphQL.Subscriptions
{
    [ExtendObjectType(OperationTypeNames.Subscription)]
    public class NotificationSubscriptions
    {
        private readonly ITopicEventReceiver _topicEventReceiver;
        private readonly IJwtService _jwtService;

        public NotificationSubscriptions(
            ITopicEventReceiver topicEventReceiver,
            ITopicEventSender topicEventSender,
            IJwtService jwtService)
        {
            _topicEventReceiver = topicEventReceiver;
            _jwtService = jwtService;
        }

        [SubscribeAndResolve]
        [Topic("notification")]
        public ValueTask<ISourceStream<SubscriptionsNotificationResponse>> Notification(string token)
        {
            if (_jwtService.ValidateToken(token, out var jwtToken))
            {
                if (int.TryParse(jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value, out int userId))
                {
                    var topic = $"notification_{userId}";

                    return _topicEventReceiver.SubscribeAsync<string, SubscriptionsNotificationResponse>(topic);
                }
            }

            return new ValueTask<ISourceStream<SubscriptionsNotificationResponse>>();
        }
    }
}
