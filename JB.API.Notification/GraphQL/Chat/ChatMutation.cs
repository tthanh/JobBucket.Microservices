using JB.Notification.Models.Notification;
using JB.Notification.Services;
using System.Threading.Tasks;
using JB.Notification.DTOs.Chat;
using HotChocolate;
using HotChocolate.Resolvers;
using AutoMapper;
using JB.Notification.Models.Chat;
using JB.Infrastructure.Models;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models.Authentication;

namespace JB.Notification.GraphQL.Chat
{
    public class ChatMutation
    {
        private readonly IMapper _mapper;
        private readonly IChatService _chatService;
        private readonly IUserClaimsModel _claims;
        public ChatMutation(
            IMapper mapper,
            IChatService chatService,
            IUserClaimsModel claims)
        {
            _mapper = mapper;
            _claims = claims;
            _chatService = chatService;
        }

        public async Task<ConversationResponse> AddOrGet(IResolverContext context, [GraphQLName("conversation")] AddConversationRequest convRequest)
        {
            Status status = new();
            ChatConversationModel conv = null;
            ChatMessageModel message = null;
            ConversationResponse result = null;

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                if (convRequest.ReceiverId > 0)
                {
                    // Get or create new conversation
                    (status, conv) = await _chatService.GetConversationByUserIds(_claims.Id, convRequest.ReceiverId);
                    if (!status.IsSuccess)
                    {
                        break;
                    }

                    result = _mapper.Map<ChatConversationModel, ConversationResponse>(conv);
                }
                else
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<MessageResponse> AddMessage(IResolverContext context, [GraphQLName("message")] AddMessageRequest messageRequest)
        {
            Status status = new();
            ChatConversationModel conv = null;
            ChatMessageModel message = null;
            MessageResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(messageRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                message = _mapper.Map<ChatMessageModel>(messageRequest);

                if (messageRequest.ConversationId == null || messageRequest.ConversationId < 0)
                {
                    if (messageRequest.ReceiverId > 0)
                    {
                        // Get or create new conversation
                        (status, conv) = await _chatService.GetConversationByUserIds(_claims.Id, messageRequest.ReceiverId.Value);
                        if (!status.IsSuccess)
                        {
                            break;
                        }

                        message.ConversationId = conv.Id;
                    }
                    else
                    {
                        status.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }
                }
                
                (status, message) = await _chatService.AddMessage(message);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }

        public async Task<NotificationModel> DeleteConversation(IResolverContext context, int id)
        {
            return null;
        }
    }
}
