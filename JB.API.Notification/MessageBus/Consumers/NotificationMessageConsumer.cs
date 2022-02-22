using AutoMapper;
using JB.Infrastructure.Messages;
using JB.Notification.Models.Notification;
using JB.Notification.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        private IServiceProvider _serviceProvider;
        
        public NotificationMessageConsumer(IMapper mapper,
            IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }
        public async Task OnHandle(NotificationMessage message, string path)
        {
            using var scope = _serviceProvider.CreateScope();

            var notiService = scope.ServiceProvider.GetService<INotificationService>();
            var noti = _mapper.Map<NotificationModel>(message);

            await notiService.Add(noti);
        }
    }
}
   