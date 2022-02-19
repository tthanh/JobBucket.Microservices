using AutoMapper;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Services;
using JB.Job.Data;
using JB.Job.Models.Job;
using JB.Job.Models.Organization;
using JB.Job.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace JB.Job.Services.Job
{
    public class JobSearchService : ISearchService<JobModel>
    {
        private readonly JobDbContext _jobDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<JobSearchService> _logger;
        private readonly IUserClaimsModel _claims;

        private readonly IUserManagementService _userService;
        private readonly IOrganizationService _organizationService;

        public JobSearchService(
            JobDbContext jobDbContext,
            IMapper mapper,
            ILogger<JobSearchService> logger,
            IUserClaimsModel claims,
            IUserManagementService userService,
            IOrganizationService organizationService
        )
        {
            _jobDbContext = jobDbContext;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _userService = userService;
            _organizationService = organizationService;
        }
        public async Task<(Status, List<JobModel>)> Search(string keyword, Expression<Func<JobModel, bool>> filter, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var jobs = new List<JobModel>();
            int userId = _claims?.Id ?? 0;

            do
            {

                try
                {
                    filter = filter.And(j =>
                        j.Title.Contains(keyword) ||
                        j.Description.Contains(keyword)
                    );

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

        public async Task<(Status, List<JobModel>)> Search(int[] entityIds, Expression<Func<JobModel, bool>> filter, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending = false)
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
    }
}
