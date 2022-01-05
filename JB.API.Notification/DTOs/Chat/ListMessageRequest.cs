using JB.Infrastructure.DTOs;
using JB.Notification.Models.Chat;
using System;
using System.Linq.Expressions;

namespace JB.Notification.DTOs.Chat
{
    public class ListMessageRequest : ListVM<ChatMessageModel>
    {
        public int? Type { get; set; }

        public Expression<Func<ChatMessageModel, object>> GetSortExpression()
        {
            return u => u.CreatedDate;
        }
    }
}
