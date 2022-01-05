using HotChocolate.Subscriptions;
using JB.Notification.Models.Chat;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using JB.Infrastructure.Models;
using JB.Infrastructure.Constants;

namespace JB.Notification.Services
{
    public static class ChatSubscriptionsExtensions
    {
        public static IApplicationBuilder SubScribeToChat(this IApplicationBuilder app)
        {
            var obserable = app.ApplicationServices.GetRequiredService<IChatSubscriptionsService>();
            var observers = app.ApplicationServices.GetServices<IObserver<ChatMessageModel>>();

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

    public interface IChatSubscriptionsService : IObservable<ChatMessageModel>
    {
        Status Add(ChatMessageModel entity);
    }

    public class ChatSubscriptionsService : IChatSubscriptionsService
    {
        private readonly List<IObserver<ChatMessageModel>> _notiObservers;

        private readonly ITopicEventSender _topicEventSender;
        public ChatSubscriptionsService(ITopicEventSender topicEventSender)
        {
            _notiObservers = new List<IObserver<ChatMessageModel>>();
            _topicEventSender = topicEventSender;
        }

        public Status Add(ChatMessageModel entity)
        {
            foreach (var o in _notiObservers)
            {
                o.OnNext(entity);
            }

            return new Status(ErrorCode.Success);
        }

        public IDisposable Subscribe(IObserver<ChatMessageModel> observer)
        {
            if (!_notiObservers.Contains(observer))
                _notiObservers.Add(observer);

            return new Unsubscriber(_notiObservers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<ChatMessageModel>> _observers;
            private IObserver<ChatMessageModel> _observer;

            public Unsubscriber(List<IObserver<ChatMessageModel>> observers, IObserver<ChatMessageModel> observer)
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
