using AutoMapper;
using JB.Blog.Models.User;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Blog.Services
{
    public class UserManagementGRPCService : IUserManagementService
    {
        private readonly ILogger<UserManagementGRPCService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;
        private readonly gRPC.User.UserRPC.UserRPCClient _userGrpcClient;

        public UserManagementGRPCService(
            ILogger<UserManagementGRPCService> logger,
            IDistributedCache cache,
            IMapper mapper,
            gRPC.User.UserRPC.UserRPCClient userGrpcClient
            )
        {
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
            _userGrpcClient = userGrpcClient;
        }

        public async Task<(Status, UserModel)> GetUser(int userId)
        {
            Status status = new Status();
            var user = await _cache.GetAsync<UserModel>($"user-{userId}");

            if (user == null)
            {
                var req = new gRPC.User.UserRequest();
                req.Id.Add(userId);

                var userResp = await _userGrpcClient.GetAsync(req);
                user = userResp.Users.Count == 1 ? _mapper.Map<UserModel>(userResp.Users[0]) : null;
            }

            return (status, user);
        }
    }
}
