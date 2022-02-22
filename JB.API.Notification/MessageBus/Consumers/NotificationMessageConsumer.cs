using AutoMapper;
using JB.Infrastructure.Messages;
using JB.Notification.Models.Notification;
using JB.Notification.Services;
using SlimMessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.API.Notification.MessageBus.Consumers
{
    public class NotificationMessageConsumer : IConsumer<NotificationMessage>
    {
        private IMapper _mapper;
        private INotificationService _notiService;
        public NotificationMessageConsumer(IMapper mapper,
            INotificationService notiService)
        {
            _mapper = mapper;
            _notiService = notiService;
        }
        public async Task OnHandle(NotificationMessage message, string path)
        {
            var noti = _mapper.Map<NotificationModel>(message);

            await _notiService.Add(noti);
        }
    }
}
   