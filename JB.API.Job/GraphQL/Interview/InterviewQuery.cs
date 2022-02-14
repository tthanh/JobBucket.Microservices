using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using JB.Job.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JB.Job.DTOs.Job;
using JB.Job.DTOs.Interview;
using JB.Job.Models.Interview;
using AutoMapper;
using System.Linq.Expressions;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Helpers;

namespace JB.Job.GraphQL.Interview
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class InterviewQuery
    {
        private readonly IMapper _mapper;
        private readonly IInterviewService _interviewService;
        private readonly IUserClaimsModel _claims;
        public InterviewQuery(
            IMapper mapper,
            IInterviewService interviewService,
            IUserClaimsModel claims)
        {
            _mapper = mapper;
            _claims = claims;
            _interviewService = interviewService;
        }

        [GraphQLName("interviews")]
        public async Task<List<InterviewResponse>> Interviews(IResolverContext context, int? id, ListInterviewRequest filterRequest)
        {
            List<InterviewResponse> results = new();
            List<InterviewModel> interviews = new();
            InterviewModel interview = null;
            Status status = new();

            do
            {
                if (id > 0)
                {
                    (status, interview) = await _interviewService.GetById(id.Value);
                    if (status.IsSuccess)
                    {
                        results = new List<InterviewResponse>()
                        {
                            _mapper.Map<InterviewResponse>(interview),
                        };
                    }

                    break;
                }

                Expression<Func<InterviewModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<InterviewModel>();
                Expression<Func<InterviewModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.Id);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 1 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? false;

                (status, interviews) = await _interviewService.List(filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                results = interviews.ConvertAll(x => _mapper.Map<InterviewResponse>(x));
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
