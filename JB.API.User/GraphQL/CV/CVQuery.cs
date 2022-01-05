using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using JB.User.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using JB.User.DTOs.CV;
using JB.User.Models.CV;
using System.Linq.Expressions;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Models;

namespace JB.User.GraphQL.CV
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class CVQuery
    {
        private readonly IMapper _mapper;
        private readonly ICVService _cvService;
        private readonly IUserClaimsModel _claims;
        public CVQuery(
            IMapper mapper,
            ICVService cvService,
            IUserClaimsModel claims)
        {
            _mapper = mapper;
            _claims = claims;
            _cvService = cvService;
        }

        [GraphQLName("cv")]
        public async Task<List<CVResponse>> CV(IResolverContext context, int? id, [GraphQLName("filter")] ListCVRequest filterRequest)
        {
            List<CVResponse> results = new();
            List<CVModel> cvs = new();
            CVModel cv = null;
            Status status = new();

            do
            {
                if (id > 0)
                {
                    (status, cv) = await _cvService.GetById(id.Value);
                    if (status.IsSuccess)
                    {
                        results = new List<CVResponse>()
                        {
                            _mapper.Map<CVResponse>(cv),
                        };
                    }

                    break;
                }

                Expression<Func<CVModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<CVModel>();
                Expression<Func<CVModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.Id);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 1 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? false;

                (status, cvs) = await _cvService.List(filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                results = cvs.ConvertAll(x => _mapper.Map<CVResponse>(x));
            }
            while (false);


            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return results;
        }
    }
}
