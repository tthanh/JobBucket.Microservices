using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using JB.Organization.Services;
using JB.Organization.DTOs.Organization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JB.Organization.Models;
using System.Linq.Expressions;

using JB.Organization.Models.Organization;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;

namespace JB.Organization.GraphQL.Organization
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class OrganizationQuery
    {
        private readonly IOrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly IUserClaimsModel _claims;
        public OrganizationQuery(
            IOrganizationService organizationService,
            IMapper mapper,
            IUserClaimsModel claims
            )
        {
            _claims = claims;
            _mapper = mapper;
            _organizationService = organizationService;
        }

        [GraphQLName("organizations")]
        public async Task<List<OrganizationResponse>> Organizations(IResolverContext context, int? id,ListOrganizationRequest filterRequest)
        {
            List<OrganizationResponse> results = new();
            List<OrganizationModel> organizations = new();
            OrganizationModel organization = null;
            Status status = new();


            do
            {
                Expression<Func<OrganizationModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<OrganizationModel>();
                Expression<Func<OrganizationModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.Id);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 1 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? false;

                if (id > 0)
                {
                    (status, organization) = await _organizationService.GetById(id.Value);
                    if (status.IsSuccess)
                    {
                        results = new List<OrganizationResponse>()
                        {
                            _mapper.Map<OrganizationResponse>(organization),
                        };

                    }

                    break;
                }

               

                (status, organizations) = await _organizationService.List(filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                results = organizations.ConvertAll(x => _mapper.Map<OrganizationResponse>(x));
            }
            while (false);


            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return results;
        }

        [GraphQLName("organizationEmployersDetail")]
        public async Task<OrganizationDetailResponse> OrganizationEmployersDetail(IResolverContext context, int? orgId)
        {
            OrganizationDetailResponse result = new();
            OrganizationModel organization = null;
            Status status = new();

            if(orgId > 0)
            {
                (status, organization) = await _organizationService.GetDetailById(orgId.Value);
                if (status.IsSuccess)
                {
                    result = _mapper.Map<OrganizationDetailResponse>(organization);
                    return result;
                }
            }
          

            

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
    }
}
