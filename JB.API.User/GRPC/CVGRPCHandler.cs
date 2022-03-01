using AutoMapper;
using Grpc.Core;
using JB.gRPC.CV;
using JB.gRPC.Profile;
using JB.User.Models.CV;
using JB.User.Models.Profile;
using JB.User.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.User.GRPC
{
    public class CVGRPCHandler : CVRPC.CVRPCBase
    {
        private readonly ICVService _cvService;
        private readonly IMapper _mapper;
        public CVGRPCHandler(
            ICVService cvService,
            IMapper mapper
            )
        {
            _cvService = cvService;
            _mapper = mapper;
        }

        public override async Task<CVResponse> Get(CVRequest request, ServerCallContext context)
        {
            CVResponse cvResponse = new CVResponse();
            Expression<Func<CVModel, bool>> filter = _ => false;

            if (request.Id.Count > 0)
            {
                filter = x => request.Id.ToArray().Contains(x.Id);
            }

            (var status, var cvs) = await _cvService.List(filter, x => x.Id, int.MaxValue, 1, false);

            if (status.IsSuccess)
            {
                cvResponse.Cvs.AddRange(_mapper.Map<List<CVModel>, List<gRPC.CV.CV>>(cvs));
            }

            return cvResponse;
        }
    }
}
