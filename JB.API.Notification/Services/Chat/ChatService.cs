using AutoMapper;
using JB.Notification.Data;
using JB.Notification.Models.Chat;
using JB.Notification.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;

namespace JB.Notification.Services
{
    public class ChatService : IChatService
    {
        private readonly ChatDbContext _chatDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatService> _logger;
        private readonly IUserClaimsModel _claims;
        private readonly IUserManagementService _userService;
        private readonly IOrganizationService _organizationService;
        private readonly IChatSubscriptionsService _chatSubscriptionsService;

        public ChatService(
            ChatDbContext chatDbContext,
            IMapper mapper,
            ILogger<ChatService> logger,
            IUserClaimsModel claims,
            IUserManagementService userService,
            IOrganizationService organizationService,
            IChatSubscriptionsService chatSubscriptionsService
        )
        {
            _chatDbContext = chatDbContext;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _userService = userService;
            _organizationService = organizationService;
            _chatSubscriptionsService = chatSubscriptionsService;
        }

        public async Task<Status> Add(ChatConversationModel entity)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            List<UserModel> users = null;

            do
            {
                try
                {
                    if (entity == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }
                    
                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    (result, users) = await _userService.GetUsers(entity.UserIds.ToList());
                    if (!result.IsSuccess || users == null || users.Count != entity.UserIds.Count())
                    {
                        break;
                    }

                    await _chatDbContext.Conversations.AddAsync(entity);
                    await _chatDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public Task<(Status, long)> Count(Expression<Func<ChatConversationModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<(Status, ChatConversationModel)> GetById(int id)
        {
            Status result = new Status();
            ChatConversationModel conv = null;

            do
            {
                try
                {
                    if (id <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    conv = await _chatDbContext.Conversations.FirstOrDefaultAsync(x => x.Id == id);
                    if (conv == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    (var getUserStatus, var users) = await _userService.GetUsers(new List<int>(conv.UserIds));
                    if (getUserStatus.IsSuccess)
                    {
                        conv.Users = users;
                    }

                    var orgId = conv.Users?.Where(x => x.OrganizationId > 0).Select(x => x.OrganizationId).FirstOrDefault();
                    if (orgId > 0)
                    {
                        (var getOrgStatus, var org) = await _organizationService.GetById(orgId.Value);
                        if (getOrgStatus.IsSuccess)
                        {
                            conv.Organization = org;
                        }
                    }
                    conv.LastMessage = await _chatDbContext.Messages.OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync(x => x.ConversationId == conv.Id);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, conv);
        }

        public async Task<(Status, List<ChatConversationModel>)> List(Expression<Func<ChatConversationModel, bool>> filter, Expression<Func<ChatConversationModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var convs = new List<ChatConversationModel>();
            int userId = _claims?.Id ?? 0;

            do
            {

                try
                {
                    var chatQuery = _chatDbContext.Conversations.Where(filter);
                    chatQuery = isDescending ? chatQuery.OrderByDescending(sort) : chatQuery.OrderBy(sort);
                    convs = await chatQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (convs == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    foreach (var conv in convs)
                    {
                        (var getUserStatus, var users) = await _userService.GetUsers(new List<int>(conv.UserIds));
                        if (getUserStatus.IsSuccess)
                        {
                            conv.Users = users;
                        }

                        var orgId = conv.Users?.Where(x => x.OrganizationId > 0).Select(x => x.OrganizationId).FirstOrDefault();
                        if (orgId > 0)
                        {
                            (var getOrgStatus, var org) = await _organizationService.GetById(orgId.Value);
                            if (getOrgStatus.IsSuccess)
                            {
                                conv.Organization = org;
                            }
                        }
                        conv.LastMessage = await _chatDbContext.Messages.OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync(x => x.ConversationId == conv.Id);
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, convs);
        }

        public async Task<(Status, ChatMessageModel)> AddMessage(ChatMessageModel chat)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            List<UserModel> users = null;

            do
            {
                try
                {
                    if (string.IsNullOrEmpty(chat.Content) || chat.Type < 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    chat.SenderId = userId;

                    await _chatDbContext.Messages.AddAsync(chat);
                    await _chatDbContext.SaveChangesAsync();

                    (var getUserStatus, var user) = await _userService.GetUser(chat.SenderId);
                    if (getUserStatus.IsSuccess)
                    {
                        chat.Sender = user;
                    }

                    Task.Run(() => _chatSubscriptionsService.Add(chat));
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, chat);
        }

        public async Task<(Status, List<ChatMessageModel>)> ListMessages(int conversationId, Expression<Func<ChatMessageModel, bool>> filter, Expression<Func<ChatMessageModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            ChatConversationModel conv = null;
            List<ChatMessageModel> messages = null;

            do
            {

                try
                {
                    conv = await _chatDbContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);
                    if (conv == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    if (!conv.UserIds.Contains(_claims.Id))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    filter = filter.And(x => x.ConversationId == conversationId);

                    var chatQuery = _chatDbContext.Messages.Where(filter);
                    chatQuery = isDescending ? chatQuery.OrderByDescending(sort) : chatQuery.OrderBy(sort);
                    messages = await chatQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (messages == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, messages);
        }

        public Task<Status> Update(ChatConversationModel entity)
        {
            throw new NotImplementedException();
        }

        public async Task<(Status, ChatConversationModel)> GetConversationByUserIds(params int[] userIds)
        {
            Status result = new Status();
            ChatConversationModel conv = null;

            do
            {
                try
                {
                    if (userIds == null || userIds.Length == 0 || userIds.Any(u => u < 0))
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    conv = await _chatDbContext.Conversations.FirstOrDefaultAsync(x => x.UserIds.All(x => userIds.Contains(x)));
                    if (conv == null)
                    {
                        conv = new ChatConversationModel
                        {
                            UserIds = userIds.OrderBy(x => x).ToArray(),
                        };

                        await _chatDbContext.Conversations.AddAsync(conv);
                        await _chatDbContext.SaveChangesAsync();
                        break;
                    }

                    (var getUserStatus, var users) = await _userService.GetUsers(new List<int>(conv.UserIds));
                    if (getUserStatus.IsSuccess)
                    {
                        conv.Users = users;
                    }

                    var orgId = conv.Users?.Where(x => x.OrganizationId > 0).Select(x => x.OrganizationId).FirstOrDefault();
                    if (orgId > 0)
                    {
                        (var getOrgStatus, var org) = await _organizationService.GetById(orgId.Value);
                        if (getOrgStatus.IsSuccess)
                        {
                            conv.Organization = org;
                        }
                    }
                    conv.LastMessage = await _chatDbContext.Messages.OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync(x => x.ConversationId == conv.Id);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, conv);
        }
    }
}
