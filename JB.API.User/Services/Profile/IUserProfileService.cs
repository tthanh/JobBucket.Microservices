﻿using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using JB.User.Models.Profile;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using JB.User.DTOs.Profile;

namespace JB.User.Services
{
    public interface IUserProfileService : IServiceBase<UserProfileModel>
    {
        Task<(Status, List<UserProfileModel>)> Search(string keyword, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
        Task<(Status, List<UserProfileModel>)> GetRecommendations(int[] entityIds = null, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
        Task<(Status, List<UserProfileModel>)> GetRecommendations(ListUserProfileRequest filter = null);
        public Task<(Status, UserProfileModel)> GetOrCreate(int id);
        Task<Status> Reindex();
    }
}
