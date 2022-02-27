using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using JB.Notification.Models.Chat;
using JB.Notification.Models.Notification;
using JB.Notification.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.GraphQL.Notification
{
    public class ChatFirebaseObserver : IObserver<ChatMessageModel>
    {
        public ChatFirebaseObserver()
        {
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ChatMessageModel value)
        {
            //var registrationToken = "AAAA3nQTKt4:APA91bHhsxeyUMGOo83PkFmaz78MVRjj9mGp-jgT4lvRLYzzy-3IJk1MRKHFNUpgCuhwlxQd_IEdYljl20DXdOZBAAOlJUQmr9yN86lsfgL54t3S6yI9FEIU-nnY-bJNKn8gQRkyOBhb";

            //// See documentation on defining a message payload.
            //var message = new Message()
            //{
            //    Data = new Dictionary<string, string>()
            //    {
            //        { "score", "850" },
            //        { "time", "2:45" },
            //        { "content", "from asp.net API" },
            //    },
            //    Token = registrationToken,
            //};

            //var s = FirebaseMessaging.DefaultInstance.SendAsync(message).GetAwaiter().GetResult();
            //FirebaseMessaging.DefaultInstance.SendAsync(message).ContinueWith(methodResult =>
            //{
            //    Console.WriteLine(methodResult.Result);
            //});
        }
    }
}
