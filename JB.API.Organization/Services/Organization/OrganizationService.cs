using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Organization.Data;
using JB.Organization.Helpers;
using JB.Organization.Models.Organization;
using JB.Organization.Models.User;
using JB.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.API.Infrastructure.Constants;

namespace JB.Organization.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly ILogger<OrganizationService> _logger;
        private readonly IUserClaimsModel _claims;
        private readonly IUserManagementService _userService;
        private readonly IJobService _jobService;
        private readonly IDistributedCache _cache;

        public OrganizationService(
            OrganizationDbContext organizationDbContext,
            ILogger<OrganizationService> logger,
            IUserClaimsModel claims,
            IUserManagementService userService,
            IDistributedCache cache,
            IJobService jobService)
        {
            _organizationDbContext = organizationDbContext;
            _logger = logger;
            _claims = claims;
            _userService = userService;
            _cache = cache;
            _jobService = jobService;
        }

        public async Task<Status> Add(OrganizationModel org)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            do
            {
                if (org == null)
                {
                    result.ErrorCode = ErrorCode.OrganizationNull;
                    break;
                }

                int organizationId = _userService.GetUser(userId).Result.Item2?.OrganizationId ?? 0;
                if (organizationId > 0)
                {
                    result.ErrorCode = ErrorCode.UserIsRecruiter;
                    break;
                }

                org.EmployerIds = new int[] { userId };
                org.ManagerIds = new int[] { userId };

                try
                {

                    await _organizationDbContext.Organizations.AddAsync(org);
                    await _organizationDbContext.SaveChangesAsync();

                    _organizationDbContext.Entry(org).State = EntityState.Detached;
                    org = await _organizationDbContext.Organizations.FindAsync(org.Id);

                    await _userService.PromoteTo(userId, org.Id, (int)RoleType.OrganizationManager);

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

        public async Task<(Status, long)> Count(Expression<Func<OrganizationModel, bool>> predicate)
        {
            Status result = new Status();
            long orgCount = 0;
            do
            {
                try
                {
                    if (predicate == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    orgCount = await _organizationDbContext.Organizations.Where(predicate).CountAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, orgCount);
        }

        public async Task<Status> Delete(int orgId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (orgId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }


                try
                {
                    var org = await _organizationDbContext.Organizations.FirstOrDefaultAsync(x => x.Id == orgId);
                    if (org == null)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }
                    if (!UserHelper.IsManager(userId, org))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _organizationDbContext.Organizations.Remove(org);
                    await _organizationDbContext.SaveChangesAsync();

                    await _cache.RemoveAsync(CacheKeys.ORGANIZATION, orgId);
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

        public async Task<(Status, OrganizationModel)> GetById(int organizationId)
        {
            Status result = new Status();
            OrganizationModel org = null;
            bool isSetCache = false;

            do
            {
                if (organizationId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
                try
                {
                    org = await _cache.GetAsync<OrganizationModel>(CacheKeys.ORGANIZATION, organizationId);
                    //org = await _cache.GetAsync<OrganizationModel>($"organization-{organizationId}");
                    if (org != null)
                    {
                        org.IsReviewAllowed = (await _jobService.IsAnyApplication(_claims.Id)).Item2;
                        break;
                    }
                    else
                    {
                        isSetCache = true;
                    }

                    org = await _organizationDbContext.Organizations.Where(x => x.Id == organizationId).FirstOrDefaultAsync();
                    if (org == null)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }

                    org.IsReviewAllowed = (await _jobService.IsAnyApplication(_claims.Id)).Item2;

                    if (isSetCache)
                    {
                        await _cache.SetAsync<OrganizationModel>($"organization-{organizationId}", org, new DistributedCacheEntryOptions
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

            return (result, org);
        }

        public async Task<(Status, List<OrganizationModel>)> GetByIds(List<int> organizationIds)
        {
            Status result = new Status();
            List<OrganizationModel> orgs = new();
            List<int> notCachedIds = new List<int>();
            do
            {
                if (organizationIds == null ||  organizationIds.Count == 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                try
                {
                    foreach (var id in organizationIds)
                    {
                        var org = await _cache.GetAsync<OrganizationModel>(CacheKeys.ORGANIZATION, id);
                        //var org = await _cache.GetAsync<OrganizationModel>($"organization-{id}");
                        if (org == null)
                        {
                            notCachedIds.Add(id);
                        }

                        orgs.Add(org);
                    }

                    if (notCachedIds.Count > 0)
                    {
                        var orgsFromDb = await _organizationDbContext.Organizations.Where(x => notCachedIds.Contains(x.Id)).ToListAsync();

                        foreach (var org in orgsFromDb)
                        {
                            await _cache.SetAsync<OrganizationModel>($"organization-{org.Id}", org, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                                SlidingExpiration = TimeSpan.FromHours(1),
                            });
                        }

                        orgs.AddRange(orgsFromDb);
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, orgs);
        }

        public async Task<(Status, OrganizationModel)> GetDetailById(int organizationId)
        {
            Status result = new Status();
            OrganizationModel organization = null;
            int userId = _claims?.Id ?? 0;

            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (organizationId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
                try
                {
                    var org = await _organizationDbContext.Organizations.Where(x => x.Id == organizationId).FirstOrDefaultAsync();
                    if (org == null)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }
                    if (!UserHelper.IsRecruiter(userId, org) && !UserHelper.IsManager(userId, org))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    organization = org;
                    if (organization.ManagerIds != null && organization.ManagerIds.Count() > 0)
                    {
                        organization.Managers = new List<UserModel>();
                        foreach (var id in organization.ManagerIds)
                            if (id > 0)
                            {
                                UserModel employer = _userService.GetUser(id).Result.Item2;
                                if (employer != null)
                                    organization.Managers.Add(employer);
                            }
                    }
                    if (organization.EmployerIds != null && organization.EmployerIds.Count() > 0)
                    {
                        organization.Employers = new List<UserModel>();
                        foreach (var id in organization.EmployerIds)
                            if (id > 0)
                            {
                                UserModel employer = _userService.GetUser(id).Result.Item2;
                                if (employer != null)
                                    organization.Employers.Add(employer);
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

            return (result, organization);
        }

        public async Task<(Status, List<OrganizationModel>)> List(Expression<Func<OrganizationModel, bool>> filter, Expression<Func<OrganizationModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var orgs = new List<OrganizationModel>();
            int userId = _claims?.Id ?? 0;

            do
            {

                try
                {

                    var orgQuery = _organizationDbContext.Organizations.Where(filter);
                    orgQuery = isDescending ? orgQuery.OrderByDescending(sort) : orgQuery.OrderBy(sort);
                    orgs = await orgQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (orgs == null)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
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

            return (result, orgs);
        }

        public async Task<Status> Update(OrganizationModel entity)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    if (entity == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }


                    var organization = await _organizationDbContext.Organizations.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();
                    if (!UserHelper.IsManager(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    PropertyHelper.InjectNonNull<OrganizationModel>(organization, entity);
                    _organizationDbContext.Update(organization);
                    await _organizationDbContext.SaveChangesAsync();

                    await _cache.RemoveAsync(CacheKeys.ORGANIZATION, organization.Id);
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

        //Organization Manager Authorized only
        #region Organization Employer Management
        public async Task<(Status, UserModel)> AddEmployer(string name, string password, string email)
        {

            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            UserModel userModel = null;
            do
            {
                try
                {
                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }
                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }
                    var organization = await GetOrganization(userId);
                    if (!UserHelper.IsManager(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    var user = new UserModel
                    {
                        Name = name,
                        UserName = email,
                        Email = email,
                        RoleId = (int)RoleType.Recruiter,
                        OrganizationId = organization.Id,
                        PasswordPlain = password
                    };
                    (var status, var userResult) = await _userService.CreateUser(user);
                    if (status.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.EmailAlreadyRegistered;
                        break;
                    }
                    userModel = userResult;

                    organization.EmployerIds = organization.EmployerIds.Append(userResult.Id).ToArray();
                    _organizationDbContext.Update(organization);
                    await _organizationDbContext.SaveChangesAsync();

                    await _cache.RemoveAsync(CacheKeys.ORGANIZATION, organization.Id);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, userModel);
        }
        public async Task<Status> DeleteEmployer(int EmployerId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            do
            {
                try
                {

                    if (userId <= 0 || EmployerId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }
                    var organization = await GetOrganization(userId);
                    if (!UserHelper.IsManager(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    if (UserHelper.IsManager(EmployerId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    if (!UserHelper.IsRecruiter(EmployerId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    var status = await _userService.DeleteUser(EmployerId);
                    if (!status.IsSuccess)
                    {
                        result.ErrorCode = ErrorCode.Unknown;
                    }
                    organization.EmployerIds = organization.EmployerIds.Where(e => e != EmployerId).ToArray();

                    _organizationDbContext.Update(organization);
                    await _organizationDbContext.SaveChangesAsync();

                    await _cache.RemoveAsync(CacheKeys.ORGANIZATION, organization.Id);
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
        public async Task<(Status, string)> ResetPassEmployer(int EmployerId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            string newPass = null;
            do
            {
                try
                {

                    if (userId <= 0 || EmployerId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }
                    var organization = await GetOrganization(userId);
                    if (!UserHelper.IsManager(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    if (!UserHelper.IsRecruiter(EmployerId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    newPass = UserHelper.GenRandomPassword(8);
                    //var status = await _userService.ResetPass(EmployerId, pass);
                    //if (!status.IsSuccess)
                    //{
                    //    result.ErrorCode = ErrorCode.Unknown;
                    //    pass = null;
                    //}

                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, newPass);
        }

        public async Task<(Status, UserModel)> PromoteEmployer(int employerId)
        {
            Status result = new Status();
            UserModel employer = null;
            int userId = _claims?.Id ?? 0;
            do
            {
                try
                {

                    if (userId <= 0 || employerId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }
                    var organization = await GetOrganization(userId);
                    if (!UserHelper.IsManager(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    if (UserHelper.IsManager(employerId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    if (!UserHelper.IsRecruiter(employerId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }


                    organization.ManagerIds.Append(employerId).ToArray();

                    _organizationDbContext.Update(organization);
                    await _organizationDbContext.SaveChangesAsync();
                    
                    await _cache.RemoveAsync(CacheKeys.ORGANIZATION, organization.Id);

                    Status userStatus = null;
                    (userStatus, employer) = await _userService.PromoteTo(employerId, organization.Id, (int)RoleType.OrganizationManager);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, employer);
        }
        public async Task<(Status, UserModel)> DemoteEmployer(int employerId)
        {
            Status result = new Status();
            UserModel employer = null;
            int userId = _claims?.Id ?? 0;
            do
            {
                try
                {

                    if (userId <= 0 || employerId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }
                    var organization = await GetOrganization(userId);
                    if (!UserHelper.IsManager(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    if (!UserHelper.IsManager(employerId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    if (!UserHelper.IsRecruiter(employerId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }


                    organization.ManagerIds = organization.ManagerIds.Where(m => m != employerId).ToArray();

                    _organizationDbContext.Update(organization);
                    await _organizationDbContext.SaveChangesAsync();

                    await _cache.RemoveAsync(CacheKeys.ORGANIZATION, organization.Id);
                    
                    Status userStatus = null;
                    (userStatus, employer) = await _userService.PromoteTo(employerId, organization.Id, (int)RoleType.Recruiter);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, employer);
        }
        //public async Task<(Status, List<JobModel>)> ListEmployerProductives(int employerId,
        //    Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending = false) 
        //{
        //    Status result = new Status();
        //    int userId = _claims?.Id ?? 0;
        //    List<JobModel> empJobs = new List<JobModel>();
        //    do
        //    {
        //        try
        //        {

        //            if (userId <= 0 || employerId <= 0)
        //            {
        //                result.ErrorCode = ErrorCode.UserNotExist;
        //                break;
        //            }
        //            var organization = await GetOrganization(userId);
        //            if (!UserHelper.IsManager(userId, organization))
        //            {
        //                result.ErrorCode = ErrorCode.NoPrivilege;
        //                break;
        //            }
        //            if (!UserHelper.IsRecruiter(employerId, organization) &&
        //                !UserHelper.IsManager(employerId, organization))
        //            {
        //                result.ErrorCode = ErrorCode.NoPrivilege;
        //                break;
        //            }

        //            (var status, var jobs) = await _jobService.ListJobByOrganization(organization.Id, sort, size, offset, isDescending);
        //            if(jobs.Count > 0)
        //            {
        //                jobs.RemoveAll(j => j.EmployerId != employerId);
        //                empJobs = jobs;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            result.ErrorCode = ErrorCode.Unknown;
        //            _logger.LogError(e, e.Message);
        //        }
        //    }
        //    while (false);

        //    return (result, empJobs);
        //}
        #endregion

        private async Task<OrganizationModel> GetOrganization(int userId)
        {
            (var status, var user) = await _userService.GetUser(userId);
            return await _organizationDbContext.Organizations.Where(x => x.Id == user.OrganizationId).FirstOrDefaultAsync();
        }

        public async Task<(Status, OrganizationModel)> UpdateRating(int organizationId, float rating, float ratingBenefit, float ratingLearning, float ratingCulture, float ratingWorkspace)
        {
            Status result = new Status();
            OrganizationModel org = null;
            
            do
            {
                if (organizationId <= 0 || rating > 5 || rating < 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
                try
                {
                    org = await _organizationDbContext.Organizations.Where(x => x.Id == organizationId).FirstOrDefaultAsync();
                    if (org == null)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }

                    org.Rating = rating;
                    org.RatingBenefit = ratingBenefit;
                    org.RatingLearning = ratingLearning;
                    org.RatingCulture = ratingCulture;
                    org.RatingWorkspace = ratingWorkspace;

                    await _organizationDbContext.SaveChangesAsync();
                 
                    await _cache.RemoveAsync(CacheKeys.ORGANIZATION, organizationId);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, org);
        }
    }
}
