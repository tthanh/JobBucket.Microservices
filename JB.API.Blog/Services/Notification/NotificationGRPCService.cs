using JB.API.Blog.Notification;
using JB.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace JB.Blog.Services
{
    public class NotificationGRPCService : INotificationService
    {
        public Task<Status> Add(NotificationModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, long)> Count(System.Linq.Expressions.Expression<Func<NotificationModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Delete(DateTime olderThan)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Delete(int[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, NotificationModel)> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, System.Collections.Generic.List<NotificationModel>)> List(System.Linq.Expressions.Expression<Func<NotificationModel, bool>> filter, System.Linq.Expressions.Expression<Func<NotificationModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            throw new NotImplementedException();
        }

        public Task<Status> MarkNotificationAsSeen(int[] notificationIds)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Update(NotificationModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
