using AutoMapper;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Elasticsearch.Job;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Job.Data;
using JB.Job.DTOs.Job;
using JB.Job.DTOs.Job.Property;
using JB.Job.Helpers;
using JB.Job.Models.Job;
using JB.Job.Models.Notification;
using JB.Job.Models.Organization;
using JB.Job.Models.User;
using JB.Job.Services.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public class JobService : IJobService
    {
        private readonly JobDbContext _jobDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<JobService> _logger;
        private readonly IUserClaimsModel _claims;

        private readonly IUserManagementService _userService;
        private readonly IOrganizationService _organizationService;
        private readonly ICVService _cvService;
        private readonly INotificationService _notiService;

        private readonly ISearchService<JobModel> _searchService;
        private readonly Nest.IElasticClient _elasticClient;

        public JobService(
            JobDbContext jobDbContext,
            IMapper mapper,
            ILogger<JobService> logger,
            IUserClaimsModel claims,
            IUserManagementService userService,
            IOrganizationService organizationService,
            ICVService cvService,
            INotificationService notiService,
            ISearchService<JobModel> searchService,
            Nest.IElasticClient elasticClient
        )
        {
            _jobDbContext = jobDbContext;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _userService = userService;
            _organizationService = organizationService;
            _cvService = cvService;
            _notiService = notiService;
            _searchService = searchService;
            _elasticClient = elasticClient;
        }

        #region Job
        public async Task<Status> Add(JobModel entity)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            do
            {
                if (entity == null)
                {
                    result.ErrorCode = ErrorCode.JobNull;
                    break;
                }
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }

                int organizationId = _userService.GetUser(userId).Result.Item2?.OrganizationId ?? 0;
                (var status, var organization) = await _organizationService.GetById(organizationId);
                if (!UserHelper.IsRecruiter(userId, organization))
                {
                    result.ErrorCode = ErrorCode.NoPrivilege;
                    break;
                }

                entity.EmployerId = userId;
                entity.OrganizationId = organizationId;

                try
                {
                    await _jobDbContext.Jobs.AddAsync(entity);
                    await _jobDbContext.SaveChangesAsync();

                    _jobDbContext.Entry(entity).State = EntityState.Detached;
                    entity = await _jobDbContext.Jobs.FindAsync(entity.Id);

                    if (entity.EmployerId > 0)
                    {
                        UserModel employer = _userService.GetUser(entity.EmployerId).Result.Item2;
                        if (employer != null)
                        {
                            entity.Employer = employer;
                        }
                    }

                    if (entity.OrganizationId > 0)
                    {
                        entity.Organization = organization;
                    }

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

        public async Task<(Status, long)> Count(Expression<Func<JobModel, bool>> predicate)
        {
            Status result = new Status();
            long jobsCount = 0;
            do
            {
                try
                {
                    if (predicate == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    jobsCount = await _jobDbContext.Jobs.Where(predicate).CountAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, jobsCount);
        }

        public async Task<Status> Delete(int jobId)
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
                if (jobId <= 0)
                {
                    result.ErrorCode = ErrorCode.JobNull;
                    break;
                }


                try
                {
                    var job = await _jobDbContext.Jobs.FirstOrDefaultAsync(x => x.Id == jobId);
                    if (job == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }
                    (var status, var organization) = await _organizationService.GetById(job.OrganizationId);
                    if (!UserHelper.IsRecruiter(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _jobDbContext.Jobs.Remove(job);
                    await _jobDbContext.SaveChangesAsync();

                    await DeleteDocument(jobId);
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

        public async Task<(Status, JobModel)> GetById(int jobId)
        {
            Status result = new Status();
            JobModel job = null;
            int userId = _claims?.Id ?? 0;
            do
            {

                try
                {
                    job = await _jobDbContext.Jobs.Where(x => x.Id == jobId).FirstOrDefaultAsync();
                    if (job == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    if (userId > 0)
                    {
                        job.IsJobInterested = job.Interests.Any(i => i.UserId == userId);
                        job.IsJobApplied = job.Applications.Any(i => i.UserId == userId);
                    }

                    if (job.EmployerId > 0)
                    {
                        UserModel employer = _userService.GetUser(job.EmployerId).Result.Item2;
                        if (employer != null)
                        {
                            job.Employer = employer;
                        }
                    }

                    if (job.OrganizationId > 0)
                    {
                        OrganizationModel organization = _organizationService.GetById(job.OrganizationId).Result.Item2;
                        if (organization != null)
                        {
                            job.Organization = organization;
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

            return (result, job);
        }

        public async Task<(Status, List<JobModel>)> List(Expression<Func<JobModel, bool>> filter, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var jobs = new List<JobModel>();
            int userId = _claims?.Id ?? 0;

            do
            {

                try
                {

                    var jobQuery = _jobDbContext.Jobs.Where(filter);
                    jobQuery = isDescending ? jobQuery.OrderByDescending(sort) : jobQuery.OrderBy(sort);
                    jobs = await jobQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (jobs == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    if (userId > 0)
                    {
                        jobs.ForEach(j =>
                        {
                            j.IsJobInterested = j.Interests.Any(i => i.UserId == userId);
                            j.IsJobApplied = j.Applications.Any(i => i.UserId == userId);
                        });
                    }

                    foreach (var job in jobs)
                    {
                        if (job.EmployerId > 0)
                        {
                            UserModel employer = _userService.GetUser(job.EmployerId).Result.Item2;
                            job.Employer = employer ?? job.Employer;
                        }

                        if (job.OrganizationId > 0)
                        {
                            OrganizationModel organization = _organizationService.GetById(job.OrganizationId).Result.Item2;
                            job.Organization = organization ?? job.Organization;
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

            return (result, jobs);
        }

        public async Task<(Status, List<JobModel>)> ListJobByOrganization(int organizationId, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var jobs = new List<JobModel>();
            int userId = _claims?.Id ?? 0;

            do
            {
                if (organizationId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                try
                {

                    var jobQuery = _jobDbContext.Jobs.Where(j => j.OrganizationId == organizationId);
                    jobQuery = isDescending ? jobQuery.OrderByDescending(sort) : jobQuery.OrderBy(sort);
                    jobs = await jobQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (jobs == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    if (userId > 0)
                    {
                        jobs.ForEach(j =>
                        {
                            j.IsJobInterested = j.Interests.Any(i => i.UserId == userId);
                            j.IsJobApplied = j.Applications.Any(i => i.UserId == userId);
                        });
                    }

                    foreach (var job in jobs)
                    {
                        if (job.EmployerId > 0)
                        {
                            UserModel employer = _userService.GetUser(job.EmployerId).Result.Item2;
                            job.Employer = employer ?? job.Employer;
                        }

                        if (job.OrganizationId > 0)
                        {
                            OrganizationModel organization = _organizationService.GetById(job.OrganizationId).Result.Item2;
                            job.Organization = organization ?? job.Organization;
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

            return (result, jobs);
        }

        public async Task<Status> Update(JobModel entity)
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
                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }


                    (var status, var organization) = await _organizationService.GetById(entity.OrganizationId);
                    if (!UserHelper.IsRecruiter(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    var job = await _jobDbContext.Jobs.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();
                    PropertyHelper.InjectNonNull<JobModel>(job, entity);
                    _jobDbContext.Update(job);
                    await _jobDbContext.SaveChangesAsync();

                    if (entity.EmployerId > 0)
                    {
                        UserModel employer = _userService.GetUser(entity.EmployerId).Result.Item2;
                        if (employer != null)
                        {
                            entity.Employer = employer;
                        }
                    }

                    if (entity.OrganizationId > 0)
                    {
                        entity.Organization = organization;
                    }
                    
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
        #endregion

        #region Interest
        public async Task<(Status, List<JobModel>)> GetInterestedJobsByUser()
        {
            Status result = new Status();
            List<JobModel> jobs = new List<JobModel>();
            var userId = _claims?.Id ?? 0;

            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }

                try
                {

                    List<InterestModel> interests = await _jobDbContext.Interests.Where(x => x.UserId == userId).ToListAsync();
                    interests.ForEach(async i =>
                    {
                        (var status, JobModel job) = await GetById(i.JobId);
                        if (status.IsSuccess)
                        {
                            jobs.Add(job);
                        }
                    });
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, jobs);
        }
        public async Task<(Status, long)> CountInterestedUsersByJob(int jobId)
        {
            Status result = new Status();
            var userId = _claims?.Id ?? 0;
            int userCount = 0;

            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (jobId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                try
                {
                    var job = await _jobDbContext.Jobs.FirstOrDefaultAsync(x => x.Id == jobId);
                    if (job == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    userCount = job.Interests.Count;
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, userCount);
        }
        public async Task<Status> AddInterestedJob(int jobId)
        {
            Status result = new Status();
            var userId = _claims?.Id ?? 0;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (jobId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                try
                {
                    bool isJobExist = await _jobDbContext.Jobs.AnyAsync(j => j.Id == jobId);
                    if (!isJobExist)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }
                    bool isInterested = await _jobDbContext.Interests.AnyAsync(i => i.UserId == userId && i.JobId == jobId);
                    if (isInterested)
                    {
                        result.ErrorCode = ErrorCode.JobAlreadyInterested;
                        break;
                    }

                    await _jobDbContext.Interests.AddAsync(new InterestModel { JobId = jobId, UserId = userId });
                    await _jobDbContext.SaveChangesAsync();
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
        public async Task<Status> RemoveInterestedJob(int jobId)
        {
            Status result = new Status();
            var userId = _claims?.Id ?? 0;

            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (jobId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
                try
                {
                    bool isJobExist = await _jobDbContext.Jobs.AnyAsync(j => j.Id == jobId);
                    if (!isJobExist)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    var interest = await _jobDbContext.Interests.FirstOrDefaultAsync(i => i.UserId == userId && i.JobId == jobId);
                    if (interest == null)
                    {
                        result.ErrorCode = ErrorCode.JobNotInterested;
                        break;
                    }

                    _jobDbContext.Interests.Remove(interest);
                    await _jobDbContext.SaveChangesAsync();
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
        #endregion

        #region Apply
        public async Task<(Status, ApplicationModel)> Apply(ApplicationModel applyForm)
        {
            Status result = new Status();
            var userId = _claims?.Id ?? 0;

            do
            {
                if (applyForm == null)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }

                if (applyForm.JobId <= 0 || (applyForm.CVId <= 0 && string.IsNullOrEmpty(applyForm.CVPDFUrl)))
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                try
                {
                    var job = await _jobDbContext.Jobs.FirstOrDefaultAsync(j => j.Id == applyForm.JobId);
                    if (job == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    if (job.ExpireDate < DateTime.UtcNow)
                    {
                        result.ErrorCode = ErrorCode.JobExpired;
                        break;
                    }

                    if (applyForm.CVId > 0)
                    {
                        (var getCVStatus, var cv) = await _cvService.GetById(applyForm.CVId);
                        if (!getCVStatus.IsSuccess)
                        {
                            result = getCVStatus;
                            break;
                        }
                    }

                    bool isApplied = await _jobDbContext.Application.AnyAsync(i => i.UserId == userId && i.JobId == applyForm.JobId);
                    if (isApplied)
                    {
                        result.ErrorCode = ErrorCode.JobAlreadyApplied;
                        break;
                    }

                    applyForm.UserId = userId;
                    await _jobDbContext.Application.AddAsync(applyForm);
                    await _jobDbContext.SaveChangesAsync();

                    await _notiService.Add(new NotificationModel
                    {
                        Message = $"User applied to job:{job.Id} - {job.Title}",
                        OrganizationId = job.OrganizationId,
                        SenderId = job.EmployerId,
                        ReceiverId = job.EmployerId,
                    });
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, applyForm);
        }

        public async Task<(Status, ApplicationModel)> Unapply(int jobId)
        {
            Status result = new Status();
            var userId = _claims?.Id ?? 0;
            ApplicationModel applyForm = null;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (jobId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                try
                {
                    var isJobExist = await _jobDbContext.Jobs.AnyAsync(j => j.Id == jobId);
                    if (!isJobExist)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    applyForm = await _jobDbContext.Application.FirstOrDefaultAsync(i => i.UserId == userId && i.JobId == jobId);
                    if (applyForm == null)
                    {
                        result.ErrorCode = ErrorCode.JobNotApplied;
                        break;
                    }

                    _jobDbContext.Application.Remove(applyForm);
                    await _jobDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, applyForm);
        }

        public async Task<(Status, List<ApplicationModel>)> ListApply(Expression<Func<ApplicationModel, bool>> filter, Expression<Func<ApplicationModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            List<ApplicationModel> applications = null;
            var userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    var appQuery = _jobDbContext.Application.Where(filter);
                    appQuery = isDescending ? appQuery.OrderByDescending(sort) : appQuery.OrderBy(sort);
                    applications = await appQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (applications == null)
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
            return (result, applications);
        }
        public async Task<(Status, List<ApplicationModel>)> GetAppliedJobsByUser(Expression<Func<ApplicationModel, bool>> filter = null, Expression<Func<ApplicationModel, object>> sort = null, int size = 30, int offset = 1, bool isDescending = false)
        {
            Status result = new Status();
            List<ApplicationModel> applications = null;
            var userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    if (userId < 0)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    filter ??= ExpressionHelper.True<ApplicationModel>();
                    sort ??= (u => u.Status);
                    size = size > 0 ? size : 30;
                    offset = offset > 0 ? offset : 1;

                    filter = filter.And(x => x.UserId == userId);

                    (result, applications) = await ListApply(filter, sort, size, offset, isDescending);
                    if (!result.IsSuccess)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
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

            return (result, applications);
        }

        public async Task<(Status, List<ApplicationModel>)> GetAppliedUsersByJob(int jobId, Expression<Func<ApplicationModel, bool>> filter = null, Expression<Func<ApplicationModel, object>> sort = null, int size = 30, int offset = 1, bool isDescending = false)
        {
            Status result = new();
            List<ApplicationModel> applications = null;
            var userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    if (jobId < 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    if (userId < 0 || 
                        (_claims.RoleId != (int)RoleType.Recruiter && _claims.RoleId != (int)RoleType.OrganizationManager))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    filter ??= ExpressionHelper.True<ApplicationModel>();
                    sort ??= (u => u.Status);
                    size = size > 0 ? size : 30;
                    offset = offset > 0 ? offset : 1;

                    filter = filter.And(x => x.JobId == jobId);

                    (result, applications) = await ListApply(filter, sort, size, offset, isDescending);
                    if (!result.IsSuccess)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    var organizationId = _userService.GetUser(userId).Result.Item2?.OrganizationId ?? 0;
                    (var status, var organization) = await _organizationService.GetById(organizationId);
                    if (!UserHelper.IsRecruiter(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    foreach (var a in applications)
                    {
                        (var getUserStatus, UserModel user) = await _userService.GetUser(a.UserId);
                        if (status.IsSuccess)
                        {
                            a.User = user;
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

            return (result, applications);
        }
        #endregion

        #region Properties
        public async Task<(Status, JobPropertiesResponse)> GetJobProperties()
        {
            Status result = new Status();
            JobPropertiesResponse properties = null;

            do
            {
                try
                {
                    var skills = await _jobDbContext.Skillls.ToListAsync();
                    var categories = await _jobDbContext.Categories.ToListAsync();
                    var positions = await _jobDbContext.Positions.ToListAsync();
                    var types = await _jobDbContext.Types.ToListAsync();

                    properties = new JobPropertiesResponse
                    {
                        Skills = skills.ConvertAll(p => new JobSkillResponse { Id = p.Id, Name = p.Name }),
                        Categories = categories.ConvertAll(p => new JobCategoryResponse { Id = p.Id, Name = p.Name }),
                        Positions = positions.ConvertAll(p => new JobPositionResponse { Id = p.Id, Name = p.Name }),
                        Types = types.ConvertAll(p => new JobTypeResponse { Id = p.Id, Name = p.Name }),
                    };
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, properties);
        }

        public async Task<(Status, List<JobModel>)> Search(string keyword, Expression<Func<JobModel, bool>> filter = null, Expression<Func<JobModel, object>> sort = null, int size = 0, int offset = 0, bool isDescending = false)
        {
            return await _searchService.Search(keyword, filter, sort, size, offset, isDescending);
        }
        #endregion

        #region
        private async Task AddDocument(JobModel job)
        {
            JobDocument doc = _mapper.Map<JobDocument>(job);
            await _elasticClient.IndexAsync(doc, r => r.Index("job"));
        }

        private async Task UpdateDocument(JobModel job)
        {
            JobDocument doc = _mapper.Map<JobDocument>(job);
            await _elasticClient.UpdateAsync<JobDocument>(job.Id, u => u.Index("job").Doc(doc));
        }

        private async Task DeleteDocument(int id)
        {
            await _elasticClient.DeleteAsync<JobModel>(id, r => r.Index("job"));
        }

        public async Task<(Status, JobCountsResponse)> GetJobCountsByCategory(int count)
        {
            Status result = new Status();
            JobCountsResponse counts = new JobCountsResponse();
            
            do
            {
                try
                {
                    counts.TotalCount = await _jobDbContext.Jobs.LongCountAsync();
                    counts.ByCategories = await _jobDbContext.Categories.Select(x => new JobCountsByResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        TotalCount = x.Jobs.LongCount(),
                    }).OrderByDescending(x => x.TotalCount).Take(count).ToListAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, counts);
        }

        public async Task<(Status, List<JobModel>)> GetRecommendations(JobModel entity, Expression<Func<JobModel, bool>> filter = null, Expression<Func<JobModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false)
        {
            return await _searchService.Search(entity, filter, sort, size, offset, isDescending);
        }

        Task<(Status, List<JobModel>)> IJobService.Search(string keyword, Expression<Func<JobModel, bool>> filter, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending)
        {
            throw new NotImplementedException();
        }

        Task<(Status, List<JobModel>)> IJobService.GetRecommendations(JobModel entity, Expression<Func<JobModel, bool>> filter, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending)
        {
            throw new NotImplementedException();
        }

        Task<(Status, List<JobModel>)> IJobService.ListJobByOrganization(int organizationId, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending)
        {
            throw new NotImplementedException();
        }

        Task<(Status, List<JobModel>)> IJobService.GetInterestedJobsByUser()
        {
            throw new NotImplementedException();
        }

        Task<(Status, long)> IJobService.CountInterestedUsersByJob(int jobId)
        {
            throw new NotImplementedException();
        }

        Task<Status> IJobService.AddInterestedJob(int jobId)
        {
            throw new NotImplementedException();
        }

        Task<Status> IJobService.RemoveInterestedJob(int jobId)
        {
            throw new NotImplementedException();
        }

        Task<(Status, ApplicationModel)> IJobService.Apply(ApplicationModel applyForm)
        {
            throw new NotImplementedException();
        }

        Task<(Status, ApplicationModel)> IJobService.Unapply(int jobId)
        {
            throw new NotImplementedException();
        }

        Task<(Status, List<ApplicationModel>)> IJobService.GetAppliedJobsByUser(Expression<Func<ApplicationModel, bool>> filter, Expression<Func<ApplicationModel, object>> sort, int size, int offset, bool isDescending)
        {
            throw new NotImplementedException();
        }

        Task<(Status, List<ApplicationModel>)> IJobService.GetAppliedUsersByJob(int jobId, Expression<Func<ApplicationModel, bool>> filter, Expression<Func<ApplicationModel, object>> sort, int size, int offset, bool isDescending)
        {
            throw new NotImplementedException();
        }

        Task<(Status, List<ApplicationModel>)> IJobService.ListApply(Expression<Func<ApplicationModel, bool>> filter, Expression<Func<ApplicationModel, object>> sort, int size, int offset, bool isDescending)
        {
            throw new NotImplementedException();
        }

        Task<(Status, JobPropertiesResponse)> IJobService.GetJobProperties()
        {
            throw new NotImplementedException();
        }

        Task<(Status, JobCountsResponse)> IJobService.GetJobCountsByCategory(int count)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
