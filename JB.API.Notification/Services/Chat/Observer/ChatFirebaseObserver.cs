using Google.Protobuf.WellKnownTypes;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using JB.Infrastructure.DTOs.Subscriptions;
using JB.Notification.Models.Chat;
using JB.Notification.Models.Notification;
using JB.Notification.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JB.Notification.GraphQL.Notification
{
    public class ChatFirebaseObserver : IObserver<ChatMessageModel>
    {
        private const string GOOGLE_FCM_API = "https://fcm.googleapis.com/fcm/send";

        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<int, int[]> _conversationUsers;

        public ChatFirebaseObserver(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _conversationUsers = new Dictionary<int, int[]>();
            _serviceProvider = serviceProvider;
        }

        public ChatFirebaseObserver(IServiceProvider serviceProvider)
        {
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
                SendNotification(value, r);
            }
        }

        private void SendNotification(ChatMessageModel value, int receiverId)
        {
            using var client = new HttpClient();

            var json = JsonConvert.SerializeObject(new
{
                to = string.Format(_configuration["GoogleFCM:Chat"], receiverId),
                priority = "high",
                notification = new
{
                    title = $"{value.Sender?.Name} sent a message",
                    body = value.Content,
                    sound = "default"
                }
            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(GOOGLE_FCM_API),
                Content = data
            };
            request.Headers.TryAddWithoutValidation("Authorization", $"key={_configuration["GoogleFCM:Key"]}");

            var result = client.Send(request);
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

        private async Task<int[]> GetConversationReceiver(int conversationId, int senderId)
        {
            var convUsers = await GetConversationUsers(conversationId);

            return convUsers?.Where(x => x != senderId).ToArray() ?? Array.Empty<int>();
        }
    }
}
