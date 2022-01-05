﻿using JB.Infrastructure.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.Blog.Models.User;

namespace JB.Blog.Services
{
    public class UserManagementGRPCService : IUserManagementService
    {
        private readonly ILogger<UserManagementGRPCService> _logger;
        private readonly IDistributedCache _cache;

        public UserManagementGRPCService(
            ILogger<UserManagementGRPCService> logger,
            IDistributedCache cache
            )
        {
            _logger = logger;
            _cache = cache;
        }

        public Task<(Status, int)> CountUser(Expression<Func<UserModel, bool>> filters)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, UserModel)> CreateUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        public Task<Status> DeleteUser(UserModel user)
        {
            throw new NotImplementedException();
        }

        public Task<Status> DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, UserModel)> DeleteUserDefaultCV(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, List<UserModel>)> DemoteToUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, UserModel)> GetUser(int userId)
        {
            throw new NotImplementedException();
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

        public Task<(Status, UserModel)> PromoteTo(int userId, int ogId, int roleId)
        {
            throw new NotImplementedException();
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
