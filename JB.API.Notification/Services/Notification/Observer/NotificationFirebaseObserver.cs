using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using JB.Notification.Models.Notification;
using JB.Notification.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JB.Notification.GraphQL.Notification
{
    public class NotificationFirebaseObserver : IObserver<NotificationModel>
    {
        private const string GOOGLE_FCM_API = "https://fcm.googleapis.com/fcm/send";

        private readonly IConfiguration _configuration;

        public NotificationFirebaseObserver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public NotificationFirebaseObserver()
        {
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(NotificationModel value)
        {
            using var client = new HttpClient();

            var json = JsonConvert.SerializeObject(new
            {
                to = string.Format(_configuration["GoogleFCM:Notification"], value.ReceiverId),
                priority = "high",
                notification = new
                {
                    title = value.Message,
                    body = value.Message,
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
    }
}
