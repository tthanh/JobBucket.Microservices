using AutoMapper;
using Grpc.Core;
using JB.gRPC.Job;
using JB.Job.Models.Job;
using JB.Job.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.GRPC
{
    public class JobGRPCHandler : JobRPC.JobRPCBase
    {
        private readonly IJobService _jobService;
        private readonly IMapper _mapper;
        public JobGRPCHandler(
            IJobService jobService,
            IMapper mapper
            )
        {
            _jobService = jobService;
            _mapper = mapper;
        }

        public override async Task<JobResponse> Get(JobRequest request, ServerCallContext context)
        {
            JobResponse jobResponse = new JobResponse();
            Expression<Func<JobModel, bool>> filter = _ => false;
            bool isSetCache = false;

            if (request.Id.Count > 0)
            {
                filter = x => request.Id.ToArray().Contains(x.Id);
            }
            
            if (request.EmployerId.Count > 0)
            {
                filter = x => request.Id.ToArray().Contains(x.EmployerId);
            }

            (var status, var jobs) = await _jobService.List(filter, x => x.Id, int.MaxValue, 1, false);

            if (status.IsSuccess)
            {
                jobResponse.Jobs.AddRange(_mapper.Map<List<JobModel>, List<gRPC.Job.Job>>(jobs));
            }

            return jobResponse;
        }

        public override async Task<CheckApplicationResponse> IsAnyApplication(CheckApplicationRequest request, ServerCallContext context)
        {
            CheckApplicationResponse appResponse = new CheckApplicationResponse();

            (var status, var app) = await _jobService.IsAnyApplication(request.Id);

            appResponse.IsAnyApplication = app;

            return appResponse;
        }
    }
}
