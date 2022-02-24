using AutoMapper;
using JB.Infrastructure.DTOs.Subscriptions;
using JB.Notification.Models.Chat;
using JB.Notification.Services;
using Microsoft.Extensions.DependencyInjection;
using SlimMessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.GraphQL.Notification
{
    public class ChatRedisPubSubObserver : IObserver<ChatMessageModel>
    {
        private readonly IMessageBus _messageBus;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<int,int[]> _conversationUsers;

        public ChatRedisPubSubObserver(
            IServiceProvider serviceProvider,
            IMessageBus messageBus,
            IMapper mapper)
        {
            _conversationUsers = new Dictionary<int, int[]>();
            _serviceProvider = serviceProvider;
            _messageBus = messageBus;
            _mapper = mapper;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ChatMessageModel value)
        {
            var receivers = GetConversationReceiver(value.ConversationId).GetAwaiter().GetResult();
            foreach (var r in receivers)
            {
                SubscriptionsMessageResponse messageResponse = _mapper.Map<SubscriptionsMessageResponse>(value);
                messageResponse.ReceiverId = r;
                _messageBus.Publish(messageResponse);
            }
        }

        private async Task<int[]> GetConversationUsers(int conversationId)
        {
            int[] convUsers = Array.Empty<int>();

            if (!_conversationUsers.ContainsKey(conversationId))
{
                using var scope = _serviceProvider.CreateScope();
                IChatService chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

                (var status, var conv) = await chatService.GetById(conversationId);
                if (status.IsSuccess)
                {
                    _conversationUsers[conversationId] = conv.UserIds;
                }

            }

            convUsers = _conversationUsers[conversationId];

            return convUsers;
        }

        private async Task<int[]> GetConversationReceiver(int conversationId)
        {
            var convUsers = await GetConversationUsers(conversationId);

            return convUsers.ToArray();
        }
    }
}
