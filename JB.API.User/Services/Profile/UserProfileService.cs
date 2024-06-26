﻿using AutoMapper;
using JB.API.Infrastructure.Constants;
using JB.gRPC.CV;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Elasticsearch.Job;
using JB.Infrastructure.Elasticsearch.User;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Services;
using JB.User.Data;
using JB.User.DTOs.Profile;
using JB.User.Models.Profile;
using JB.User.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.User.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ProfileDbContext _profileDbContext;
        private readonly IUserManagementService _userManagementService;
        private readonly IOrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;
        private readonly IUserClaimsModel _claims;
        private readonly IUserProfileSearchService _searchService;
        private readonly IUserProfileDocumentElasticsearchService _documentService;
        private readonly IDistributedCache _cache;

        public UserProfileService(
            ProfileDbContext profileDbContext,
            IUserManagementService userManagementService,
            IOrganizationService organizationService,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            IUserClaimsModel claims,
            IUserProfileSearchService searchService,
            IDistributedCache cache,
            IUserProfileDocumentElasticsearchService documentService)
        {
            _profileDbContext = profileDbContext;
            _userManagementService = userManagementService;
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _cache = cache;
            _documentService = documentService;
            _searchService = searchService;
        }

        public async Task<Status> Add(UserProfileModel entity)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;

            do
            {
                if (entity == null)
                {
                    result.ErrorCode = ErrorCode.cvNull;
                    break;
                }

                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.NoPrivilege;
                    break;
                }

                try
                {
                    await _profileDbContext.Profiles.AddAsync(entity);
                    await _profileDbContext.SaveChangesAsync();

                    await _documentService.AddAsync(entity);
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

        public Task<(Status, long)> Count(Expression<Func<UserProfileModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<(Status, UserProfileModel)> GetById(int id)
        {
            Status result = new Status();
            UserProfileModel profile = null;
            bool isSetCache = false;

            do
            {
                try
                {
                    profile = await _cache.GetAsync<UserProfileModel>(CacheKeys.PROFILE, id);
                    //profile = await _cache.GetAsync<UserProfileModel>($"profile-{id}");
                    if (profile != null)
                    {
                        break;
                    }
                    else
                    {
                        isSetCache = true;
                    }

                    profile = await _profileDbContext.Profiles.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (profile == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    if (isSetCache)
                    {
                        await _cache.SetAsync<UserProfileModel>($"profile-{id}", profile, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                            SlidingExpiration = TimeSpan.FromHours(1),
                        });
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, profile);
        }

        public async Task<(Status, UserProfileModel)> GetOrCreate(int id)
        {
            Status result = new Status();
            UserProfileModel profile = null;
            UserModel user = null;

            do
            {
                if (id <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }

                try
                {
                    profile = await _profileDbContext.Profiles.FirstOrDefaultAsync(p => p.Id == id);
                    if (profile == null)
                    {
                        // Init profile
                        (result, user) = await _userManagementService.GetUser(id);
                        if (!result.IsSuccess)
                        {
                            break;
                        }

                        profile = _mapper.Map<UserProfileModel>(user);

                        await _profileDbContext.Profiles.AddAsync(profile);
                        await _profileDbContext.SaveChangesAsync();

                        await _documentService.AddAsync(profile);
                    }

                    if (profile.OrganizationId > 0)
                    {
                        (var getCreatorStatus, var org) = await _organizationService.GetById(profile.OrganizationId.Value);
                        if (getCreatorStatus.IsSuccess)
                        {
                            profile.Organization = org;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, profile);
        }

        public async Task<(Status, List<UserProfileModel>)> List(Expression<Func<UserProfileModel, bool>> filter, Expression<Func<UserProfileModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var profiles = new List<UserProfileModel>();
            int userId = _claims?.Id ?? 0;
            do
            {
                try
                {
                    var cvQuery = _profileDbContext.Profiles.Where(filter);
                    cvQuery = isDescending ? cvQuery.OrderByDescending(sort) : cvQuery.OrderBy(sort);
                    profiles = await cvQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (profiles == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, profiles);
        }

        public async Task<Status> Update(UserProfileModel entity)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;

            do
            {
                if (entity == null || userId <= 0)
                {
                    result.ErrorCode = ErrorCode.cvNull;
                    break;
                }

                if (entity.Id != userId)
                {
                    result.ErrorCode = ErrorCode.NoPrivilege;
                    break;
                }

                try
                {
                    var profile = await _profileDbContext.Profiles.Where(x => x.Id == userId).FirstOrDefaultAsync();
                    PropertyHelper.InjectNonNull<UserProfileModel>(profile, entity);
                    _profileDbContext.Update(profile);
                    await _profileDbContext.SaveChangesAsync();

                    var user = _mapper.Map<UserModel>(entity);
                    await _userManagementService.UpdateUser(user);

                    await _documentService.UpdateAsync(entity);
                 
                    await _cache.RemoveAsync(CacheKeys.PROFILE, user.Id);
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

        #region
        public async Task<(Status, List<UserProfileModel>)> Search(string keyword, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false)
            => await _searchService.Search(keyword, filter, sort, size, offset, isDescending);

        public async Task<(Status, List<UserProfileModel>)> GetRecommendations(int[] entityIds = null, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false)
            => await _searchService.Search(entityIds, filter, sort, size, offset, isDescending);

        public async Task<(Status, List<UserProfileModel>)> GetRecommendations(ListUserProfileRequest filter = null)
            => await _searchService.Search(filter);
        #endregion

        public async Task<Status> Reindex()
        {
            await _documentService.DeleteIndiceAsync();

            (var status, var jobs) = await List(j => true, j => j.Id, int.MaxValue, 1, false);
            if (status.IsSuccess)
            {
                foreach (var data in jobs)
                {
                    await _documentService.AddAsync(data);
                }
            }

            return new Status();
        }
    }
}
