using JB.Infrastructure.Models;
using System;
using System.Threading.Tasks;
using JB.Infrastructure.Services;
using JB.API.Blog.Notification;

namespace JB.Blog.Services
{
    public interface INotificationService : IServiceBase<NotificationModel>
    {
        Task<Status> Delete(DateTime olderThan);
        Task<Status> Delete(int[] ids);
        Task<Status> MarkNotificationAsSeen(int[] notificationIds);
    }
}
