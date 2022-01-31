using AutoMapper;
using JB.Infrastructure.Messages;
using JB.Infrastructure.Models;
using JB.Job.Models.Notification;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SlimMessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public class NotificationRemoteService : INotificationService
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger<NotificationRemoteService> _logger;
        public NotificationRemoteService(IMessageBus messageBus,
            ILogger<NotificationRemoteService> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }
        public async Task<Status> Add(NotificationModel entity)
        {
            await _messageBus.Publish(new NotificationMessage
            {
                Data = entity.Data,
                Level = entity.Level,
                Message = entity.Message,
                OrganizationId = entity.OrganizationId,
                ReceiverId = entity.ReceiverId,
                SenderId = entity.SenderId,
                Type = entity.Type,
            });

            return new Status();
        }
    }
}
