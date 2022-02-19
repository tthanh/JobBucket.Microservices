using AutoMapper;
using Grpc.Core;
using JB.gRPC.Profile;
using JB.User.Models.Profile;
using JB.User.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.User.GRPC
{
    public class ProfileGRPCHandler : ProfileRPC.ProfileRPCBase
    {
        private readonly IUserProfileService _profileService;
        private readonly IMapper _mapper;
        public ProfileGRPCHandler(
            IUserProfileService profileService,
            IMapper mapper
            )
        {
            _profileService = profileService;
            _mapper = mapper;
        }

        public override async Task<ProfileResponse> Get(ProfileRequest request, ServerCallContext context)
        {
            ProfileResponse profileResponse = new ProfileResponse();
            Expression<Func<UserProfileModel, bool>> filter = _ => false;

            if (request.Id.Count > 0)
            {
                filter = x => request.Id.ToArray().Contains(x.Id);
            }

            (var status, var profiles) = await _profileService.List(filter, x => x.Id, int.MaxValue, 1, false);

            if (status.IsSuccess)
            {
                profileResponse.Profiles.AddRange(_mapper.Map<List<UserProfileModel>, List<gRPC.Profile.Profile>>(profiles));
            }

            return profileResponse;
        }
    }
}
