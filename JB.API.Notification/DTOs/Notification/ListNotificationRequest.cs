using JB.Notification.Models.Notification;
using System;
using System.Linq;
using System.Linq.Expressions;
using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;

namespace JB.Notification.DTOs.Notification
{
    public class ListNotificationRequest : ListVM<NotificationModel>
    {
        public int[] OrganizationId { get; set; }
        public int[] SenderId { get; set; }
        public int[] Level { get; set; }
        public int[] Type { get; set; }
        public DateTime[] CreatedDate { get; set; }

        public override Expression<Func<NotificationModel, bool>> GetFilterExpression()
        {
            Expression<Func<NotificationModel, bool>> filter = ExpressionHelper.True<NotificationModel>();

            if (OrganizationId?.Length > 0)
            {
                filter = filter.And(noti => OrganizationId.Contains(noti.OrganizationId));
            }

            if (SenderId?.Length > 0)
            {
                filter = filter.And(noti => SenderId.Contains(noti.SenderId));
            }

            if (Type?.Length > 0)
            {
                filter = filter.And(noti => Type.Contains(noti.Type));
            }

            if (Level?.Length > 0)
            {
                Array.Sort(Level);

                int lowValue = Level.ElementAtOrDefault(0);
                int highValue = Level.ElementAtOrDefault(1);

                lowValue = lowValue == default ? int.MinValue : lowValue;
                highValue = highValue == default ? int.MaxValue : highValue;

                filter = filter.And(noti => (noti.Level >= lowValue && noti.Level <= highValue));
            }

            if (CreatedDate != null)
            {
                Array.Sort(CreatedDate);

                DateTime lowValue = CreatedDate.ElementAtOrDefault(0);
                DateTime highValue = CreatedDate.ElementAtOrDefault(1);

                lowValue = lowValue == default ? DateTime.MinValue : lowValue;
                highValue = highValue == default ? DateTime.MaxValue : highValue;

                filter = filter.And(j => j.CreatedDate >= lowValue && j.CreatedDate <= highValue);
            }

            return filter;
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(NotificationModel.CreatedDate), nameof(NotificationModel.Level), nameof(NotificationModel.Type), nameof(NotificationModel.OrganizationId), nameof(NotificationModel.SenderId) };
        }
    }
}
