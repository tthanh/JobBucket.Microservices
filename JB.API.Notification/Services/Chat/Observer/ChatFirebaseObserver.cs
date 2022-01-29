using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using JB.Notification.Models.Chat;
using JB.Notification.Models.Notification;
using JB.Notification.Services;
using System;
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
        }
    }
}
