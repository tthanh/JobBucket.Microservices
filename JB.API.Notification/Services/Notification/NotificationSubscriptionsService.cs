using JB.Notification.Models.Notification;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using JB.Infrastructure.Models;
using JB.Infrastructure.Constants;

namespace JB.Notification.Services
{
    public static class NotificationSubscriptionsExtensions
    {
        public static IApplicationBuilder SubScribeToNotification(this IApplicationBuilder app)
        {
            var obserable = app.ApplicationServices.GetRequiredService<INotificationSubscriptionsService>();
            var observers = app.ApplicationServices.GetServices<IObserver<NotificationModel>>();

            if (obserable != null && observers != null)
            {
                foreach (var o in observers)
                {
                    obserable.Subscribe(o);
                }
            }

            return app;
        }
    }

    public interface INotificationSubscriptionsService : IObservable<NotificationModel>
    {
        Status Add(NotificationModel entity);
    }

    public class NotificationSubscriptionsService : INotificationSubscriptionsService
    {
        private readonly List<IObserver<NotificationModel>> _notiObservers;
        public NotificationSubscriptionsService()
        {
            _notiObservers = new List<IObserver<NotificationModel>>();
        }

        public Status Add(NotificationModel entity)
        {
            foreach (var o in _notiObservers)
            {
                o.OnNext(entity);
            }

            return new Status(ErrorCode.Success);
        }

        public IDisposable Subscribe(IObserver<NotificationModel> observer)
        {
            if (!_notiObservers.Contains(observer))
                _notiObservers.Add(observer);

            return new Unsubscriber(_notiObservers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<NotificationModel>> _observers;
            private IObserver<NotificationModel> _observer;

            public Unsubscriber(List<IObserver<NotificationModel>> observers, IObserver<NotificationModel> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }

}
