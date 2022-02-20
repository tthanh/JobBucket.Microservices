using AutoMapper;
using JB.API.User.Models.Job;
using JB.Infrastructure.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JB.Infrastructure.Helpers;
using Status = JB.Infrastructure.Models.Status;

namespace JB.User.Services
{
    public class JobGRPCService : IJobService
    {
        private readonly ILogger<JobGRPCService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;
        private readonly gRPC.Job.JobRPC.JobRPCClient _jobGrpcClient;
        public JobGRPCService(
            ILogger<JobGRPCService> logger,
            IDistributedCache cache,
            IMapper mapper,
            gRPC.Job.JobRPC.JobRPCClient jobGrpcClient
            )
        {
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
            _jobGrpcClient = jobGrpcClient;
        }
        public async Task<(Status, List<JobModel>)> ListByEmployerId(int employerId)
        {
            Status status = new Status();
            var jobs = await _cache.GetAsync<List<JobModel>>($"job-employer-{employerId}");

            if (jobs == null)
            {
                var req = new gRPC.Job.JobRequest();
                req.EmployerId.Add(employerId);
                
                var jobResp = await _jobGrpcClient.GetAsync(req);
                jobs = jobResp.Jobs.Select(j => _mapper.Map<JobModel>(j)).ToList();
            }

            return (status, jobs);
        }

        public async Task<(Status, List<JobModel>)> ListByIds(int[] ids)
        {
            throw new NotImplementedException();
        }
    }
}
