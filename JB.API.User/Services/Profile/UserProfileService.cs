using AutoMapper;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Elasticsearch.User;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Services;
using JB.User.Data;
using JB.User.Models.Profile;
using JB.User.Models.User;
using Microsoft.EntityFrameworkCore;
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
        private readonly ISearchService<UserProfileModel> _searchService;
        private readonly Nest.IElasticClient _elasticClient;

        public UserProfileService(
            ProfileDbContext profileDbContext,
            IUserManagementService userManagementService,
            IOrganizationService organizationService,
            IMapper mapper,
            ILogger<UserProfileService> logger,
            IUserClaimsModel claims,
            ISearchService<UserProfileModel> searchService,
            Nest.IElasticClient elasticClient
            )
        {
            _profileDbContext = profileDbContext;
            _userManagementService = userManagementService;
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _searchService = searchService;
            _elasticClient = elasticClient;
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

                    await AddDocument(entity);
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

            do
            {
                try
                {
                    profile = await _profileDbContext.Profiles.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (profile == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
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

                        await AddDocument(profile);
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

                    await UpdateDocument(entity);
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
        private async Task AddDocument(UserProfileModel profile)
        {
            UserProfileDocument doc = _mapper.Map<UserProfileDocument>(profile);
            await _elasticClient.IndexAsync(doc, r => r.Index("profile"));
        }

        private async Task UpdateDocument(UserProfileModel profile)
        {
            UserProfileDocument doc = _mapper.Map<UserProfileDocument>(profile);
            await _elasticClient.UpdateAsync<UserProfileDocument>(profile.Id, u => u.Index("profile").Doc(doc));
        }

        private async Task DeleteDocument(int id)
        {
            await _elasticClient.DeleteAsync<UserProfileModel>(id, r => r.Index("profile"));
        }

        public async Task<(Status, List<UserProfileModel>)> Search(string keyword, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false)
            => await _searchService.Search(keyword, filter, sort, size, offset, isDescending);

        public async Task<(Status, List<UserProfileModel>)> GetRecommendations(int[] entityIds = null, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false)
            => await _searchService.Search(entityIds, filter, sort, size, offset, isDescending);
        #endregion
    }
}
