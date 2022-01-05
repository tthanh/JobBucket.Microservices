using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using JB.Notification.Models.Chat;
using System;
using System.Linq;
using System.Threading.Tasks;
using JB.Notification.DTOs.Chat;
using AutoMapper;
using JB.Notification.Services;

namespace JB.Notification.GraphQL.Chat
{
    [ExtendObjectType(OperationTypeNames.Subscription)]
    public class ChatSubscriptions : IObserver<ChatMessageModel>
    {
        private readonly ITopicEventSender _topicEventSender;
        private readonly ITopicEventReceiver _topicEventReceiver;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public ChatSubscriptions(
            ITopicEventSender topicEventSender,
            ITopicEventReceiver topicEventReceiver,
            IJwtService jwtService,
            IMapper mapper)
        {
            _topicEventSender = topicEventSender;
            _topicEventReceiver = topicEventReceiver;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        [SubscribeAndResolve]
        [Topic("chat")]
        public ValueTask<ISourceStream<MessageResponse>> Chat(string token)
        {
            if (_jwtService.ValidateToken(token, out var jwtToken))
            {
                if (int.TryParse(jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value, out int userId))
                {
                    var topic = $"chat_{userId}";

                    return _topicEventReceiver.SubscribeAsync<string, MessageResponse>(topic);
                }
            }

            return new ValueTask<ISourceStream<MessageResponse>>();
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ChatMessageModel value)
        {
            if (value.Conversation?.UserIds?.Length > 0)
            {
                var message = _mapper.Map<MessageResponse>(value);

                foreach (var i in value.Conversation?.UserIds)
                {
                     _topicEventSender.SendAsync($"chat_{i}", message);
                }
            }
        }
    }
}

