using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using JB.Notification.Models.Notification;
using System;
using System.Threading.Tasks;

namespace JB.Notification.Services
{
    public interface INotificationService : IServiceBase<NotificationModel>
    {
        Task<Status> Delete(DateTime olderThan);
        Task<Status> Delete(int[] ids);
        Task<Status> MarkNotificationAsSeen(int[] notificationIds);
    }
}
