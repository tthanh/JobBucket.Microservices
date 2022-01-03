using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using JB.User.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JB.User.DTOs.Profile;
using JB.User.Models.Profile;
using AutoMapper;
using System.Linq.Expressions;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;

namespace JB.User.GraphQL.Profile
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class ProfileQuery
    {
        private readonly IUserProfileService _profileService;
        private readonly IUserClaimsModel _claims;
        private readonly IMapper _mapper;
        public ProfileQuery(
            IUserProfileService profileService,
            IUserClaimsModel claims,
            IMapper mapper)
        {
            _claims = claims;
            _profileService = profileService;
            _mapper = mapper;
        }

        public async Task<List<UserProfileResponse>> Profiles(IResolverContext context, int? id, bool? myProfile, [GraphQLName("filter")] ListUserProfileRequest filterRequest)
        {
            List<UserProfileResponse> results = new();
            List<UserProfileModel> profiles = new();
            UserProfileModel profile = null;
            Status status = new();

            do
            {
                if (id == null && (myProfile ?? false))
                {
                    // Get self profile but token is invalid
                    if (_claims.Id < 0)
                    {
                        status.ErrorCode = ErrorCode.Unauthorized;
                        break;
                    }

                    id = _claims.Id;
                }

                if (id > 0)
                {
                    (status, profile) = await _profileService.GetOrCreate(id.Value);
                    if (status.IsSuccess)
                    {
                        results = new List<UserProfileResponse>()
                        {
                            _mapper.Map<UserProfileResponse>(profile),
                        };
                    }

                    break;
                }

                Expression<Func<UserProfileModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<UserProfileModel>();
                Expression<Func<UserProfileModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.Id);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 1 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? false;

                (status, profiles) = await _profileService.List(filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                results = profiles.ConvertAll(x => _mapper.Map<UserProfileResponse>(x));
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
