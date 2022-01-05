using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using JB.Notification.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JB.Notification.DTOs.Chat;
using JB.Notification.Models.Chat;
using System.Linq.Expressions;
using AutoMapper;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Models;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Constants;

namespace JB.Notification.GraphQL.Chat
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class ChatQuery
    {
        private readonly IMapper _mapper;
        private readonly IChatService _chatService;
        private readonly IUserClaimsModel _claims;
        public ChatQuery(
            IMapper mapper,
            IChatService chatService,
            IUserClaimsModel claims)
        {
            _mapper = mapper;
            _claims = claims;
            _chatService = chatService;
        }

        [GraphQLName("conversations")]
        public async Task<List<ConversationResponse>> Conversations(IResolverContext context, int? id, [GraphQLName("filter")] ListConversationRequest filterRequest)
        {
            List<ConversationResponse> results = new();
            Status status = new();
            List<ChatConversationModel> convs = new();
            ChatConversationModel conv = new();

            do
            {
                Expression<Func<ChatConversationModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<ChatConversationModel>();
                Expression<Func<ChatConversationModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.UpdatedDate);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 0 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? true;

                if (id > 0)
                {
                    (status, conv) = await _chatService.GetById(id.Value);
                    if (status.IsSuccess)
                    {
                        results = new List<ConversationResponse>()
                        {
                            _mapper.Map<ConversationResponse>(conv),
                        };
                    }

                    break;
                }

                filter = filter.And(x => x.UserIds.Contains(_claims.Id));
                (status, convs) = await _chatService.List(filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                results = convs.ConvertAll(x => _mapper.Map<ConversationResponse>(x));
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return results;
        }

        [GraphQLName("messages")]
        public async Task<List<MessageResponse>> Messages(IResolverContext context, int conversationId, [GraphQLName("filter")] ListMessageRequest filterRequest)
        {
            List<MessageResponse> results = new();
            Status status = new();
            List<ChatMessageModel> messages = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                Expression<Func<ChatMessageModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<ChatMessageModel>();
                Expression<Func<ChatMessageModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.CreatedDate);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 0 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? true;

                (status, messages) = await _chatService.ListMessages(conversationId, filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                messages.Reverse();

                results = messages.ConvertAll(x => _mapper.Map<MessageResponse>(x));
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return results;
        }
    }
}
