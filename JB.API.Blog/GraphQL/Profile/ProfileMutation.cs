using JB.User.Services;
using System.Threading.Tasks;
using JB.User.DTOs.Profile;
using HotChocolate;
using JB.User.Models.Profile;
using AutoMapper;
using HotChocolate.Resolvers;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Models;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Constants;

namespace JB.User.GraphQL.Profile
{
    public class ProfileMutation
    {
        private readonly IUserProfileService _profileService;
        private readonly IUserClaimsModel _claims;
        private readonly IMapper _mapper;
        public ProfileMutation(
            IUserProfileService profileService,
            IUserClaimsModel claims,
            IMapper mapper)
        {
            _claims = claims;
            _profileService = profileService;
            _mapper = mapper;
        }
        public async Task<UserProfileResponse> Update(IResolverContext context, [GraphQLName("profile")] UpdateUserProfileRequest profileRequest)
        {
            Status status = new();
            UserProfileModel profile = null;
            UserProfileResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(profileRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, profile) = await _profileService.GetOrCreate(_claims.Id);
                if (!status.IsSuccess)
                {
                    break;
                }

                profile = _mapper.Map(profileRequest, profile);
                if (profile == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _profileService.Update(profile);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<UserProfileResponse>(profile);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
    }
}
