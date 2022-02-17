using AutoMapper;
using JB.Notification.Data;
using JB.Notification.Models.Notification;
using JB.Notification.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.Infrastructure.Models;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models.Authentication;
using SlimMessageBus;
using JB.Infrastructure.Messages;

namespace JB.Notification.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationDbContext _notificationDbContext;
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<NotificationService> _logger;
        private readonly IOrganizationService _organizationService;
        private readonly INotificationSubscriptionsService _notificationSubscriptionsService;
        private readonly IUserClaimsModel _claims;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;

        public NotificationService(
            NotificationDbContext notificationDbContext,
            IUserManagementService userManagementService,
            IOrganizationService organizationService,
            INotificationSubscriptionsService notificationSubscriptionsService,
            ILogger<NotificationService> logger,
            IUserClaimsModel claims,
            IMapper mapper,
            IMessageBus messageBus
            )

        {
            _notificationDbContext = notificationDbContext;
            _userManagementService = userManagementService;
            _organizationService = organizationService;
            _notificationSubscriptionsService = notificationSubscriptionsService;
            _claims = claims;
            _mapper = mapper;
            _logger = logger;
            _messageBus = messageBus;
        }

        public async Task<Status> Add(NotificationModel entity)
        {
            Status result = new Status();

            do
            {
                try
                {
                    if (entity == null || entity.SenderId < 0 || entity.ReceiverId <= 0)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    await _notificationDbContext.Notifications.AddAsync(entity);
                    await _notificationDbContext.SaveChangesAsync();

                    _notificationSubscriptionsService.Add(entity);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }

        public async Task<Status> Add(NotificationModel entity, int[] receiverIds)
        {
            Status result = new Status();

            do
            {
                try
                {
                    if (entity == null || entity.SenderId < 0)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    foreach (var r in receiverIds)
                    {
                        (var getUser, var user) = await _userManagementService.GetUser(r);
                        if (!getUser.IsSuccess || user == null)
                        {
                            continue;
                        }

                        NotificationModel noti = new NotificationModel
                        {
                            Data = entity.Data,
                            Level = entity.Level,
                            OrganizationId = entity.OrganizationId,
                            SenderId = entity.SenderId,
                            Message = entity.Message,
                            Type = entity.Type,
                            ReceiverId = r,
                        };

                        await _notificationDbContext.Notifications.AddAsync(noti);
                        _notificationSubscriptionsService.Add(noti);
                    }
                    
                    await _notificationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, long)> Count(Expression<Func<NotificationModel, bool>> predicate)
        {
            Status result = new Status();
            long count = 0;

            do
            {
                try
                {
                    count = await _notificationDbContext.Notifications.Where(predicate).CountAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return (result, count);
        }

        public async Task<Status> Delete(int id)
        {
            Status result = new Status();
            NotificationModel noti = null;

            do
            {
                try
                {
                    if (id <= 0)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    if (_claims?.Id <= 0)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    noti = await _notificationDbContext.Notifications.FirstOrDefaultAsync(x => x.Id == id);
                    if (noti == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    if (noti.ReceiverId != _claims.Id)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _notificationDbContext.Notifications.Remove(noti);
                    await _notificationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }

        public async Task<Status> Delete(int[] notificationIds)
        {
            Status result = new Status();
            NotificationModel noti = null;

            do
            {
                try
                {
                    if (notificationIds == null || notificationIds.Length == 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    if (_claims?.Id <= 0)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _notificationDbContext.Notifications.RemoveRange(
                        _notificationDbContext.Notifications.Where(x => x.ReceiverId == _claims.Id && notificationIds.Contains(x.Id)));

                    await _notificationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, NotificationModel)> GetById(int id)
        {
            Status status = new Status();
            NotificationModel noti = null;
            UserModel sender = null;

            do
            {
                try
                {
                    noti = await _notificationDbContext.Notifications.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (noti == null)
                    {
                        break;
                    }

                    (status, sender) = await _userManagementService.GetUser(noti.SenderId);
                    if (status.IsSuccess)
                    {
                        noti.Sender = sender;
                    }

                    (var getOrgStatus, var org) = await _organizationService.GetById(noti.OrganizationId);
                    if (getOrgStatus.IsSuccess)
                    {
                        noti.Organization = org;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    status.ErrorCode = ErrorCode.Unknown;
                }

            }
            while (false);

            return (status, noti);
        }

        public async Task<(Status, List<NotificationModel>)> List(Expression<Func<NotificationModel, bool>> filter, Expression<Func<NotificationModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status status = new Status();
            var notifications = new List<NotificationModel>();

            do
            {
                try
                {
                    var notiQuery = _notificationDbContext.Notifications.Where(filter);
                    notiQuery = isDescending ? notiQuery.OrderByDescending(sort) : notiQuery.OrderBy(sort);
                    notifications = await notiQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (notifications == null)
                    {
                        break;
                    }

                    foreach (var noti in notifications)
                    {
                        (var getSenderStatus, var sender) = await _userManagementService.GetUser(noti.SenderId);
                        if (getSenderStatus.IsSuccess)
                        {
                            noti.Sender = sender;
                        }

                        (var getOrgStatus, var org) = await _organizationService.GetById(noti.OrganizationId);
                        if (getOrgStatus.IsSuccess)
                        {
                            noti.Sender = sender;
                        }
                    }
                }
                catch (Exception e)
                {
                    status.ErrorCode = ErrorCode.Unknown;
                }

            }
            while (false);

            return (status, notifications);
        }

        public async Task<Status> Update(NotificationModel entity)
        {
            Status result = new Status();

            do
            {
                if (entity == null)
                {
                    result.ErrorCode = ErrorCode.JobNull;
                    break;
                }

                try
                {
                    await _notificationDbContext.SaveChangesAsync();
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

        public async Task<Status> MarkNotificationAsSeen(int[] notificationIds)
        {
            Status result = new Status();

            do
            {
                try
                {
                    if (notificationIds == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }


                    foreach (var notiId in notificationIds)
                    {
                        var noti = await _notificationDbContext.Notifications.FirstOrDefaultAsync(x => x.Id == notiId);
                        if (noti != null && noti.ReceiverId == _claims.Id)
                        {
                            noti.SeenByUser = true;
                        }
                    }

                    await _notificationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }

        public async Task<Status> Delete(DateTime olderThan)
        {
            Status result = new Status();
            NotificationModel noti = null;

            do
            {
                try
                {
                    if (_claims?.Id <= 0)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _notificationDbContext.Notifications.RemoveRange(
                        _notificationDbContext.Notifications.Where(x => x.ReceiverId == _claims.Id && x.CreatedDate < olderThan));

                    await _notificationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }
    }
}
