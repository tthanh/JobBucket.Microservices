using AutoMapper;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Job.Models.Organization;
using JB.Job.Models.User;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.Infrastructure.Helpers;
using JB.API.Infrastructure.Constants;

namespace JB.Job.Services
{
    public class OrganizationGRPCService : IOrganizationService
    {
        private readonly ILogger<OrganizationGRPCService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;
        private readonly gRPC.Organization.OrganizationRPC.OrganizationRPCClient _orgGrpcClient;

        public OrganizationGRPCService(
            ILogger<OrganizationGRPCService> logger,
            IDistributedCache cache,
            IMapper mapper,
            gRPC.Organization.OrganizationRPC.OrganizationRPCClient orgGrpcClient
            )
        {
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
            _orgGrpcClient = orgGrpcClient;
        }

        public async Task<(Status, OrganizationModel)> GetById(int id)
        {
            Status status = new Status();
            var org = await _cache.GetAsync<OrganizationModel>(CacheKeys.ORGANIZATION, id);
            //var org = await _cache.GetAsync<OrganizationModel>($"organization-{id}");

            if (org == null)
            {
                var req = new gRPC.Organization.OrganizationRequest();
                req.Id.Add(id);
                var userResp = await _orgGrpcClient.GetAsync(req);
                org = userResp.Organizations.Count == 1 ? _mapper.Map<OrganizationModel>(userResp.Organizations[0]) : null;
            }

            return (status, org);
        }

    }
}
