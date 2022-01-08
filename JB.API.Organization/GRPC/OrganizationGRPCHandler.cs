using AutoMapper;
using Grpc.Core;
using JB.gRPC.Organization;
using JB.Organization.Models.Organization;
using JB.Organization.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Authentication.GRPC
{
    public class OrganizationGRPCHandler : OrganizationRPC.OrganizationRPCBase
    {
        private readonly IOrganizationService _orgService;
        private readonly IMapper _mapper;
        public OrganizationGRPCHandler(
            IOrganizationService orgService,
            IMapper mapper
            )
        {
            _orgService = orgService;
            _mapper = mapper;
        }

        public override async Task<OrganizationResponse> Get(OrganizationRequest request, ServerCallContext context)
        {
            OrganizationResponse organizationResponse = new OrganizationResponse();

            (var status, var orgs) = await _orgService.GetByIds(request.Id.ToList());
            if (status.IsSuccess)
            {
                orgs.RemoveAll(x => x == null);
                organizationResponse.Organizations.AddRange(_mapper.Map<List<OrganizationModel>, List<gRPC.Organization.Organization>>(orgs));
            }

            return organizationResponse;
        }
    }
}
