using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using JB.Notification.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Notification.Services
{
    public interface IChatService : IServiceBase<ChatConversationModel>
    {
        Task<(Status, ChatMessageModel)> AddMessage(ChatMessageModel chat);
        Task<(Status, ChatConversationModel)> GetConversationByUserIds(params int[] userIds);
        Task<(Status, List<ChatMessageModel>)> ListMessages(int conversationId, Expression<Func<ChatMessageModel, bool>> filter, Expression<Func<ChatMessageModel, object>> sort, int size, int offset, bool isDescending = false);
    }
}
