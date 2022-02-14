using JB.Notification.Models.Chat;
using System;
using System.Linq.Expressions;
using JB.Infrastructure.DTOs;

namespace JB.Notification.DTOs.Chat
{
    public class ListConversationRequest : ListVM<ChatConversationModel>, ISearchRequest
    {
        public string Keyword { get; set; }

        public override Expression<Func<ChatConversationModel, object>> GetSortExpression()
        {
            return (u => u.UpdatedDate);
        }
    }
}
