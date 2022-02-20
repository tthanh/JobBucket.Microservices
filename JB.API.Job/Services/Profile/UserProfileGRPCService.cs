using AutoMapper;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Job.Models.Organization;
using JB.Job.Models.User;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.Infrastructure.Helpers;
using JB.Job.Models.Profile;

namespace JB.Job.Services
{
    public class UserProfileGRPCService : IUserProfileService
    {
        private readonly ILogger<UserProfileGRPCService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;
        private readonly gRPC.Profile.ProfileRPC.ProfileRPCClient _profileGrpcClient;

        public UserProfileGRPCService(
            ILogger<UserProfileGRPCService> logger,
            IDistributedCache cache,
            IMapper mapper,
            gRPC.Profile.ProfileRPC.ProfileRPCClient profileGrpcClient
            )
        {
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
            _profileGrpcClient = profileGrpcClient;
        }

        public async Task<(Status, UserProfileModel)> GetById(int id)
        {
            Status status = new Status();
            var profile = await _cache.GetAsync<UserProfileModel>($"profile-{id}");

            if (profile == null)
            {
                var req = new gRPC.Profile.ProfileRequest();
                req.Id.Add(id);
                var userResp = await _profileGrpcClient.GetAsync(req);
                profile = userResp.Profiles.Count == 1 ? _mapper.Map<UserProfileModel>(userResp.Profiles[0]) : null;
                if (profile == null)
                {
                    status.ErrorCode = Infrastructure.Constants.ErrorCode.UserNotExist;
                }
            }

            return (status, profile);
        }

    }
}
