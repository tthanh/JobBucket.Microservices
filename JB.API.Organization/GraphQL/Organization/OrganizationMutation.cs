using JB.Organization.Services;
using System.Threading.Tasks;
using HotChocolate.Resolvers;
using AutoMapper;
using HotChocolate;
using JB.Organization.Models.Organization;
using JB.Organization.DTOs.Organization;
using JB.Organization.Models.User;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Models;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Constants;

namespace JB.Organization.GraphQL.Organization
{
    public class OrganizationMutation
    {
        private readonly IOrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly IUserClaimsModel _claims;
        public OrganizationMutation(
            IOrganizationService organizationService,
            IMapper mapper,
            IUserClaimsModel claims)
        {
            _claims = claims;
            _mapper = mapper;
            _organizationService = organizationService;
        }

        public async Task<OrganizationResponse> Add(IResolverContext context, [GraphQLName("organization")] AddOrganizationRequest OrganizationRequest)
        {
            Status status = new();
            OrganizationResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(OrganizationRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                var Organization = _mapper.Map<OrganizationModel>(OrganizationRequest);
                if (Organization == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _organizationService.Add(Organization);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<OrganizationResponse>(Organization);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<OrganizationResponse> Update(IResolverContext context, [GraphQLName("organization")] UpdateOrganizationRequest OrganizationRequest)
        {
            Status status = new();
            OrganizationModel organization = null;
            OrganizationResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(OrganizationRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, organization) = await _organizationService.GetById(OrganizationRequest.Id);
                if (!status.IsSuccess)
                {
                    break;
                }

                organization = _mapper.Map(OrganizationRequest, organization);
                if (organization == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _organizationService.Update(organization);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<OrganizationResponse>(organization);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<OrganizationResponse> Delete(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _organizationService.Delete(id);
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
        public async Task<AddEmployerResponse> AddEmployer(IResolverContext context, [GraphQLName("organizationEmployer")] AddEmployerRequest request)
        {
            Status status = new();
            AddEmployerResponse result = null;
            UserModel newEmployer = null;
            do
            {
                if (!PropertyHelper.TryValidateObject(request, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }



                (status, newEmployer) = await _organizationService.AddEmployer(request.Name, request.PasswordPlain, request.Email);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<AddEmployerResponse>(newEmployer);
                result.UserName = request.Email;

            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<OrganizationUserResponse> DeleteEmployer(IResolverContext context, int id)
        {
            Status status = new();
            do
            {

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _organizationService.DeleteEmployer(id);
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
        public async Task<ResetPasswordEmployerResponse> ResetPassEmployer(IResolverContext context, int id)
        {
            Status status = new();
            ResetPasswordEmployerResponse result = null;
            string strPass = null;
            do
            {

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, strPass) = await _organizationService.ResetPassEmployer(id);
                if (!status.IsSuccess)
                {
                    break;
                }
                result = new ResetPasswordEmployerResponse { PasswordPlain = strPass };

            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<OrganizationUserResponse> PromoteEmployer(IResolverContext context, int id)
        {
            Status status = new();
            OrganizationUserResponse result = null;
            do
            {

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                UserModel user = null;
                (status, user) = await _organizationService.PromoteEmployer(id);
                if (!status.IsSuccess)
                {
                    break;
                }
                result = _mapper.Map<OrganizationUserResponse>(user);

            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<OrganizationUserResponse> DemoteEmployer(IResolverContext context, int id)
        {
            Status status = new();
            OrganizationUserResponse result = null;
            do
            {

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                UserModel user = null;
                (status, user) = await _organizationService.DemoteEmployer(id);
                if (!status.IsSuccess)
                {
                    break;
                }
                result = _mapper.Map<OrganizationUserResponse>(user);

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
