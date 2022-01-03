using Grpc.Core;
using JB.gRPC.Job;
using JB.Job.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.API.Job.GRPC
{
    public class JobGRPCHandler : JB.gRPC.Job.JobRPC.JobRPCBase
    {
        private readonly ILogger<JobGRPCHandler> _logger;
        private readonly IJobService _jobService;
        public JobGRPCHandler(ILogger<JobGRPCHandler> logger,
            IJobService jobService
            )
        {
            _logger = logger;
            _jobService = jobService;
        }

        public override async Task<JobResponse> Get(JobRequest request, ServerCallContext context)
        {
            var jobResponse = new JobResponse();

            jobResponse.Jobs.AddRange(new Google.Protobuf.Collections.RepeatedField<gRPC.Job.Job>
                {
                    new gRPC.Job.Job
                    {
                        Title = "Test",
                    }
                });

            return jobResponse;
        }
    }
}
