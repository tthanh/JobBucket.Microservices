using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Status = JB.Infrastructure.Models.Status;

namespace JB.Organization.Services
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

        public async Task<(Status, bool)> IsAnyApplication(int employeeId)
        {
            Status status = new Status();

            var req = new gRPC.Job.CheckApplicationRequest
            {
                Id = employeeId,
            };

            var appResp = await _jobGrpcClient.IsAnyApplicationAsync(req);
            
            return (new Status(), appResp.IsAnyApplication);
        }
    }
}
