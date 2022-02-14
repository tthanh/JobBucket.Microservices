using JB.Notification.Models.Notification;
using JB.Notification.Services;
using System;
using System.Threading.Tasks;
using HotChocolate.Resolvers;
using JB.Notification.DTOs.Notification;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Constants;

namespace JB.Notification.GraphQL.Notification
{
    public class NotificationMutation
    {
        private readonly IUserClaimsModel _claims;
        private readonly INotificationService _notificationService;
        public NotificationMutation(
            IUserClaimsModel claims,
            INotificationService notificationService)
        {
            _claims = claims;
            _notificationService = notificationService;
        }

        public async Task<NotificationModel> Clear(IResolverContext context, int[] ids, DateTime? olderThan)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                if (olderThan != null)
                {
                    status = await _notificationService.Delete(olderThan.Value);
                    break;
                }

                if (ids != null)
                {
                    status = await _notificationService.Delete(ids);
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<NotificationModel> MarkAsSeen(IResolverContext context, int[] ids)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                if (ids != null)
                {
                    status = await _notificationService.MarkNotificationAsSeen(ids);
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<NotificationModel> Test(IResolverContext context, NotificationRequest notification)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                await _notificationService.Add(new NotificationModel
                {
                    Data = notification.Data,
                    Level = notification.Level,
                    Message = notification.Message,
                    OrganizationId = notification.OrganizationId ?? -1,
                    ReceiverId = notification.ReceiverId,
                    SenderId = _claims.Id,
                    Type = notification.Type,
                });
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
    }
}
