using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using JB.Notification.Models.Chat;
using JB.Notification.Models.Notification;
using JB.Notification.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.GraphQL.Notification
{
    public class ChatRedisPubSubObserver : IObserver<ChatMessageModel>
    {
        private readonly IJwtService _jwtService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<int,int[]> _conversationUsers;

        public ChatRedisPubSubObserver(
            IServiceProvider serviceProvider)
        {
            _conversationUsers = new Dictionary<int, int[]>();
            _serviceProvider = serviceProvider;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ChatMessageModel value)
        {
            var receivers = GetConversationReceiver(value.ConversationId, value.SenderId).GetAwaiter().GetResult();

            foreach (var r in receivers)
            {
                //Task.Run(() => _topicEventSender.SendAsync($"chat_{r}", value));
            }
        }

        private async Task<int[]> GetConversationUsers(int conversationId)
        {
            int[] convUsers = Array.Empty<int>();

            if (!_conversationUsers.ContainsKey(conversationId))
{
                using (var scope = _serviceProvider.CreateScope())
                {
                    IChatService chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

                    (var status, var conv) = await chatService.GetById(conversationId);
                    if (status.IsSuccess)
                    {
                        _conversationUsers[conversationId] = conv.UserIds;
                    }
                }

            }

            convUsers = _conversationUsers[conversationId];

            return convUsers;
        }

        private async Task<int[]> GetConversationReceiver(int conversationId, int sender)
        {
            var convUsers = await GetConversationUsers(conversationId);

            return convUsers.Where(x => x != sender).ToArray();
        }
    }
}
