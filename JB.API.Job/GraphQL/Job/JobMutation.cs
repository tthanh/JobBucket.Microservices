using JB.Job.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using HotChocolate.Resolvers;
using HotChocolate;
using AutoMapper;
using JB.Job.DTOs.Job;
using JB.Job.Models.Job;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Helpers;
using HotChocolate.Subscriptions;

namespace JB.Job.GraphQL.Job
{
    public class JobMutation
    {
        private readonly IMapper _mapper;
        private readonly IJobService _jobService;
        private readonly IUserClaimsModel _claims;
        private readonly INotificationService _notificationService;
        public JobMutation(
            IMapper mapper,
            IJobService jobService,
            IUserClaimsModel claims,
            INotificationService notificationService)
        {
            _mapper = mapper;
            _claims = claims;
            _jobService = jobService;
            _notificationService = notificationService;
        }

        public async Task<JobResponse> Add(IResolverContext context, [GraphQLName("job")] AddJobRequest jobRequest)
        {
            Status status = new();
            JobResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(jobRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                var job = _mapper.Map<JobModel>(jobRequest);
                if (job == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _jobService.Add(job);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<JobResponse>(job);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<JobResponse> Update(IResolverContext context, [GraphQLName("job")] UpdateJobRequest jobRequest)
        {
            Status status = new();
            JobModel job = null;
            JobResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(jobRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, job) = await _jobService.GetById(jobRequest.Id);
                if (!status.IsSuccess)
                {
                    break;
                }

                job = _mapper.Map(jobRequest, job);
                if (job == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _jobService.Update(job);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<JobResponse>(job);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<JobResponse> Delete(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _jobService.Delete(id);
                if (!status.IsSuccess)
                {
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<JobResponse> AddInterested(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _jobService.AddInterestedJob(id);
                if (!status.IsSuccess)
                {
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<JobResponse> RemoveInterested(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _jobService.RemoveInterestedJob(id);
                if (!status.IsSuccess)
                {
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<ApplicationResponse> Apply(IResolverContext context, [GraphQLName("application")] ApplicationRequest applicationRequest)
        {
            Status status = new();
            ApplicationModel applyForm = null;
            ApplicationResponse result = null;

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                applyForm = _mapper.Map<ApplicationModel>(applicationRequest);
                (status, applyForm) = await _jobService.Apply(applyForm);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<ApplicationResponse>(applyForm);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<ApplicationResponse> Unapply(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _jobService.Unapply(id);
                if (!status.IsSuccess)
                {
                    break;
                }

            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<ApplicationResponse> FailAplication(IResolverContext context, int jobId, int userId)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _jobService.FailApplication(jobId, userId);
                if (!status.IsSuccess)
                {
                    break;
                }

            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<ApplicationResponse> PassAplication(IResolverContext context, int jobId, int userId)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _jobService.PassApplication(jobId, userId);
                if (!status.IsSuccess)
                {
                    break;
                }

            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
    }
}
