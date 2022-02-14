using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using JB.Job.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JB.Job.DTOs.Job;
using JB.Job.Models.Job;
using System.Linq.Expressions;
using AutoMapper;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Helpers;
using JB.Job.Helpers;
using JB.Infrastructure.Constants;

namespace JB.Job.GraphQL.Job
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class JobQuery
    {
        private readonly IMapper _mapper;
        private readonly IJobService _jobService;
        private readonly IOrganizationService _orgService;
        private readonly IUserManagementService _userService;
        private readonly IUserClaimsModel _claims;
        public JobQuery(
            IMapper mapper,
            IJobService jobService,
            IOrganizationService orgService,
            IUserManagementService userService,
            IUserClaimsModel claims)
        {
            _mapper = mapper;
            _claims = claims;
            _jobService = jobService;
            _orgService = orgService;
            _userService = userService;
        }

        [GraphQLName("jobs")]
        public async Task<List<JobResponse>> Jobs(IResolverContext context, int? id, ListJobRequest filter)
        {
            List<JobResponse> results = new();
            Status status = new();
            List<JobModel> jobs = new();
            JobModel job = new();

            do
            {
                Expression<Func<JobModel, bool>> filterExpr = filter?.GetFilterExpression() ?? ExpressionHelper.True<JobModel>();
                Expression<Func<JobModel, object>> sortExpr = filter?.GetSortExpression() ?? (u => u.Id);
                int size = filter?.Size > 0 ? filter.Size.Value : 20;
                int page = filter?.Page > 0 ? filter.Page.Value : 1;
                bool isDescending = filter?.IsDescending ?? false;

                if (filter?.IsInterested == true && _claims.Id > 0)
                {
                    filterExpr = filterExpr.And(b => b.Interests.Any(x => x.UserId == _claims.Id));
                }

                if (id > 0)
                {
                    (status, job) = await _jobService.GetById(id.Value);
                    if (status.IsSuccess)
                    {
                        results = new List<JobResponse>()
                        {
                            _mapper.Map<JobResponse>(job),
                        };
                    }

                    break;
                }

                if (!string.IsNullOrEmpty(filter?.Keyword))
                {
                    (status, jobs) = await _jobService.Search(filter.Keyword, filterExpr, sortExpr, size, page, isDescending);
                    if (!status.IsSuccess)
                    {
                        break;
                    }
                }
                else
                {
                    (status, jobs) = await _jobService.List(filterExpr, sortExpr, size, page, isDescending);
                    if (!status.IsSuccess)
                    {
                        break;
                    }
                }

                results = _mapper.Map<List<JobResponse>>(jobs);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return results;
        }

        [GraphQLName("jobRecommendations")]
        public async Task<List<JobResponse>> JobRecommendations(IResolverContext context, ListJobRecommendationRequest filter)
        {
            List<JobResponse> results = new();
            Status status = new();
            List<JobModel> jobs = new();
            JobModel job = new();

            do
            {
                int size = filter?.Size > 0 ? filter.Size.Value : 20;
                int page = filter?.Page > 0 ? filter.Page.Value : 1;
                bool isDescending = filter?.IsDescending ?? false;

                (status, jobs) = await _jobService.GetRecommendations(new JobModel
                {
                    Id = filter?.JobId ?? 0,
                }, j => true, j => j.Id, size, page, isDescending);

                if (!status.IsSuccess)
                {
                    break;
                }

                results = _mapper.Map<List<JobResponse>>(jobs);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return results;
        }

        [GraphQLName("jobSearchSuggestions")]
        public async Task<List<JobSearchSuggestionResponse>> JobSearchSuggestions(IResolverContext context, int? id, JobSearchSuggestionRequest search)
        {
            return null;
        }

        [GraphQLName("jobTops")]
        public async Task<List<JobTopResponse>> JobTops(IResolverContext context, ListJobRequest filter)
        {
            return null;
        }

        [GraphQLName("jobTopSearchedKeywords")]
        public async Task<List<string>> JobTopSearchedKeywords(IResolverContext context, ListJobRequest filter)
        {
            return new List<string>
            {
                "Remote",
                "Java",
                "JavaScript",
                "Dev Ops",
                "Banking",
                "Web"
            };
        }

        [GraphQLName("jobInterests")]
        public async Task<List<JobResponse>> JobsInterested(IResolverContext context, ListJobRequest filter)
        {
            List<JobResponse> results = new();
            List<JobModel> jobs = new();
            Status status = new();

            do
            {
                Expression<Func<JobModel, bool>> filterExpr = filter?.GetFilterExpression() ?? ExpressionHelper.True<JobModel>();
                Expression<Func<JobModel, object>> sortExpr = filter?.GetSortExpression() ?? (u => u.Id);
                int size = filter?.Size > 0 ? filter.Size.Value : 20;
                int page = filter?.Page > 0 ? filter.Page.Value : 1;
                bool isDescending = filter?.IsDescending ?? false;

                (status, jobs) = await _jobService.GetInterestedJobsByUser();
                if (!status.IsSuccess)
                {
                    break;
                }

                results = jobs.ConvertAll(x => _mapper.Map<JobResponse>(x));
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return results;
        }

        [GraphQLName("jobProperties")]
        public async Task<JobPropertiesResponse> JobProperties(IResolverContext context)
        {
            (var status, var properties) = await _jobService.GetJobProperties();
            if (status.IsSuccess)
            {
                return properties;
            }

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }

        [GraphQLName("jobApplications")]
        public async Task<List<ApplicationModel>> JobApplications(IResolverContext context, ListJobApplicationRequest filter)
        {
            List<ApplicationModel> applications = new();
            Status status = new();

            do
            {
                Expression<Func<ApplicationModel, bool>> filterExpr = filter?.GetFilterExpression() ?? ExpressionHelper.True<ApplicationModel>();
                Expression<Func<ApplicationModel, object>> sortExpr = filter?.GetSortExpression() ?? (u => u.Status);
                int size = filter?.Size > 0 ? filter.Size.Value : 20;
                int page = filter?.Page > 0 ? filter.Page.Value : 1;

                bool isDescending = filter?.IsDescending ?? false;
                bool isValidFilter = false;
                if (_claims.Id < 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                if (filter?.UserId > 0)
                {
                    filterExpr = filterExpr.And(x => x.UserId == _claims.Id);
                    isValidFilter = true;
                }

                if (filter?.EmployerId > 0)
                {
                    filterExpr = filterExpr.And(x => x.Job.EmployerId == _claims.Id);
                    isValidFilter = true;
                }

                if (filter?.OrganizationId > 0)
                {
                    (var getOrgStatus, var org) = await _orgService.GetById(filter.OrganizationId.Value);
                    if (!getOrgStatus.IsSuccess)
                    {
                        status = getOrgStatus;
                        break;
                    }

                    if (!UserHelper.IsManager(_claims.Id, org))
                    {
                        status.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    isValidFilter = true;
                }

                if (filter?.JobId > 0)
                {
                    //(var getUserStatus, var user) = await _userService.GetUser(_claims.Id);
                    //if (!getUserStatus.IsSuccess)
                    //{
                    //    status = getUserStatus;
                    //    break;
                    //}

                    //if (user?.OrganizationId == null || user.OrganizationId.Value < 0)
                    //{
                    //    status.ErrorCode = ErrorCode.NoPrivilege;
                    //    break;
                    //}

                    //filterExpr = filterExpr.And(x => x.Job.OrganizationId == user.OrganizationId);
                    filterExpr = filterExpr.And(x => x.Job.Id == filter.JobId.Value);
                    isValidFilter = true;
                }

                if (isValidFilter)
                    (status, applications) = await _jobService.ListApply(filterExpr, sortExpr, size, page, isDescending);

                if (!status.IsSuccess)
                {
                    context.ReportError(status.Message);
                }
            }
            while (false);

            return applications;
        }

        [GraphQLName("jobCounts")]
        public async Task<JobCountsResponse> JobCounts(IResolverContext context, int? count)
        {
            JobCountsResponse result = new();
            Status status = new();

            do
            {
                (status, result) = await _jobService.GetJobCountsByCategory(count ?? 10);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
    }
}
