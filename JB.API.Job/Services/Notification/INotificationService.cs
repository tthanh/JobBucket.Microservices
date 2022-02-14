using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using JB.Job.Models.Notification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public interface INotificationService
    {
        Task<Status> Add(NotificationModel entity);
    }
}
