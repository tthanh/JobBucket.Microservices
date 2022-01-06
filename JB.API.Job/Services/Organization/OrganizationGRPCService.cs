using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Job.Models.Organization;
using JB.Job.Models.User;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public class OrganizationGRPCService : IOrganizationService
    {
        private readonly ILogger<OrganizationGRPCService> _logger;
        private readonly IUserClaimsModel _claims;

        public OrganizationGRPCService(
            ILogger<OrganizationGRPCService> logger,
            IUserClaimsModel claims
            )
        {
            _logger = logger;
            _claims = claims;
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

        public Task<(Status, OrganizationModel)> GetById(int id)
        {
            throw new NotImplementedException();
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
