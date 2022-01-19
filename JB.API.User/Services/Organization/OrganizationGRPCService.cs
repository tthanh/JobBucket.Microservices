using AutoMapper;
using JB.Infrastructure.Models;
using JB.User.Models.Organization;
using JB.User.Models.User;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.User.Services
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

        public Task<Status> Add(OrganizationModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, UserModel)> AddEmployer(string name, string username, string passwordPlain)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, long)> Count(Expression<Func<OrganizationModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Status> DeleteEmployer(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, UserModel)> DemoteEmployer(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<(Status, OrganizationModel)> GetById(int id)
        {
            Status status = new Status();
            var req = new gRPC.Organization.OrganizationRequest();
            req.Id.Add(id);

            var userResp = await _orgGrpcClient.GetAsync(req);
            OrganizationModel org = userResp.Organizations.Count == 1 ? _mapper.Map<OrganizationModel>(userResp.Organizations[0]) : null;

            return (status, org);
        }

        public Task<(Status, OrganizationModel)> GetDetailById(int organizationId)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, List<OrganizationModel>)> List(Expression<Func<OrganizationModel, bool>> filter, Expression<Func<OrganizationModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, UserModel)> PromoteEmployer(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, string)> ResetPassEmployer(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Update(OrganizationModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, OrganizationModel)> UpdateRating(int organizationId, float rating, float ratingBenefit, float ratingLearning, float ratingCulture, float ratingWorkspace)
        {
            throw new NotImplementedException();
        }
    }
}
