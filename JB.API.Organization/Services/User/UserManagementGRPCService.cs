using JB.Organization.Data;
using JB.Organization.Models;
using JB.Organization.Models.User;
using JB.Infrastructure.Models;
using JB.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using JB.API.Infrastructure.Constants;
using SlimMessageBus;
using JB.Infrastructure.Messages;
using static Grpc.Core.Metadata;
using JB.gRPC.User;

namespace JB.Organization.Services
{
    public class UserManagementGRPCService : IUserManagementService
    {
        private readonly ILogger<UserManagementGRPCService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;
        private readonly gRPC.User.UserRPC.UserRPCClient _userGrpcClient;
        private readonly IMessageBus _messageBus;

        public UserManagementGRPCService(
            IMessageBus messageBus,
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
            _messageBus = messageBus;
        }

        public Task<(Status, int)> CountUser(Expression<Func<UserModel, bool>> filters)
        {
            throw new NotImplementedException();
        }

        public async Task<(Status, UserModel)> CreateUser(UserModel user)
        {
            Status status = new Status();

            if (user != null)
            {
                var req = new gRPC.User.CreateUserRequest
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    RoleId = user.RoleId,
                    PasswordPlain = user.PasswordPlain,
                    OrganizationId = user.OrganizationId.Value,
                    Name = user.Name,
                };

                var userResp = await _userGrpcClient.CreateAsync(req);
                user = _mapper.Map<User, UserModel>(userResp);
            }

            return (status, user);
        }

        public Task<Status> DeleteUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        public async Task<Status> DeleteUser(int userId)
        {
            Status status = new Status();
            var user = await _cache.GetAsync<UserModel>(CacheKeys.USER, userId);

            if (user == null)
            {
                var req = new gRPC.User.UserRequest();
                req.Id.Add(userId);

                var userResp = await _userGrpcClient.DeleteAsync(req);
            }

            return status;
        }

        public Task<(Status, UserModel)> DeleteUserDefaultCV(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, List<UserModel>)> DemoteToUser(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<(Status, UserModel)> GetUser(int userId)
        {
            Status status = new Status();
            var user = await _cache.GetAsync<UserModel>(CacheKeys.USER, userId);
            //var user = await _cache.GetAsync<UserModel>($"user-{userId}");

            if (user == null)
            {
                var req = new gRPC.User.UserRequest();
                req.Id.Add(userId);

                var userResp = await _userGrpcClient.GetAsync(req);
                user = userResp.Users.Count == 1 ? _mapper.Map<UserModel>(userResp.Users[0]) : null;
            }

            return (status, user);
        }

        public Task<(Status, UserModel)> GetUser(string userName, string authSource = null)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, List<int>)> GetUserIds(Expression<Func<UserModel, bool>> filters)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, List<UserModel>)> GetUsers(List<int> userIds)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, List<UserModel>)> ListUser(Expression<Func<UserModel, bool>> filters, Expression<Func<UserModel, object>> sorts, int size, int offset)
        {
            throw new NotImplementedException();
        }

        public Task<Status> LockUser(int userId, DateTime lockUntil)
        {
            throw new NotImplementedException();
        }

        public async Task<(Status, UserModel)> PromoteTo(int userId, int ogId, int roleId)
        {
            await _messageBus.Publish(new PromoteUserMessage 
            {
                UserId = userId,
                OrganizationId = ogId,
                RoleId = roleId,
            });

            return (new Status(), null);
        }

        public Task<Status> UnlockUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, UserModel)> UpdateUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, UserModel)> UpdateUserDefaultCV(int userId, int cvId)
        {
            throw new NotImplementedException();
        }
    }
}
