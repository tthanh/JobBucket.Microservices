using AutoMapper;
using JB.Infrastructure.Models;
using JB.Job.Models.Notification;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public class NotificationGRPCService : INotificationService
    {
        public Task<Status> Add(NotificationModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, long)> Count(Expression<Func<NotificationModel, bool>> predicate)
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

        public Task<(Status, List<NotificationModel>)> List(Expression<Func<NotificationModel, bool>> filter, Expression<Func<NotificationModel, object>> sort, int size, int offset, bool isDescending = false)
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
