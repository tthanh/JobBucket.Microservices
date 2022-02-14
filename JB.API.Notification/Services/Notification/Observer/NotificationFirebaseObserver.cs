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
    public class NotificationFirebaseObserver : IObserver<NotificationModel>
    {
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
        }
    }
}
