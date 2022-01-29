using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using System.Linq;
using System.Threading.Tasks;
using JB.Gateway.DTOs.Chat;
using JB.Gateway.Services;

namespace JB.Gateway.GraphQL.Subscriptions
{
    [ExtendObjectType(OperationTypeNames.Subscription)]
    public class ChatSubscriptions
    {
        private readonly ITopicEventSender _topicEventSender;
        private readonly ITopicEventReceiver _topicEventReceiver;
        private readonly IJwtService _jwtService;

        public ChatSubscriptions(
            ITopicEventSender topicEventSender,
            ITopicEventReceiver topicEventReceiver,
            IJwtService jwtService)
        {
            _topicEventSender = topicEventSender;
            _topicEventReceiver = topicEventReceiver;
            _jwtService = jwtService;
        }

        [SubscribeAndResolve]
        [Topic("chat")]
        public ValueTask<ISourceStream<SubscriptionsMessageResponse>> Chat(string token)
        {
            if (_jwtService.ValidateToken(token, out var jwtToken))
            {
                if (int.TryParse(jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value, out int userId))
                {
                    var topic = $"chat_{userId}";

                    return _topicEventReceiver.SubscribeAsync<string, SubscriptionsMessageResponse>(topic);
                }
            }

            return new ValueTask<ISourceStream<SubscriptionsMessageResponse>>();
        }
    }
}

