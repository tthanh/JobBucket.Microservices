using HotChocolate;
using HotChocolate.Types;
using JB.Notification.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JB.Notification.DTOs.Notification;
using JB.Notification.Models.Notification;
using System.Linq.Expressions;
using AutoMapper;
using HotChocolate.Resolvers;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Helpers;

namespace JB.Notification.GraphQL.Notification
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class NotificationQuery
    {
        private readonly IMapper _mapper;
        private readonly INotificationService _notiService;
        private readonly IUserClaimsModel _claims;
        public NotificationQuery(
            IMapper mapper,
            INotificationService notiService,
            IUserClaimsModel claims)
        {
            _mapper = mapper;
            _claims = claims;
            _notiService = notiService;
        }

        [GraphQLName("notifications")]
        public async Task<List<NotificationResponse>> Notification(IResolverContext context, [GraphQLName("filter")] ListNotificationRequest filterRequest)
        {
            List<NotificationResponse> results = new();
            List<NotificationModel> notis = new();
            Status status = new();

            do
            {
                Expression<Func<NotificationModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<NotificationModel>();
                Expression<Func<NotificationModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.Id);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 1 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? false;

                filter = filter.And(x => x.ReceiverId == _claims.Id);

                (status, notis) = await _notiService.List(filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                results = notis.ConvertAll(x => _mapper.Map<NotificationResponse>(x));
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
