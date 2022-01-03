using JB.Job.Services;
using System.Threading.Tasks;
using JB.Job.DTOs.Interview;
using HotChocolate;
using HotChocolate.Resolvers;
using AutoMapper;
using JB.Job.Models.Interview;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Helpers;

namespace JB.Job.GraphQL.Interview
{
    public class InterviewMutation
    {
        private readonly IMapper _mapper;
        private readonly IInterviewService _interviewService;
        private readonly IUserClaimsModel _claims;
        public InterviewMutation(
            IMapper mapper,
            IInterviewService interviewService,
            IUserClaimsModel claims)
        {
            _mapper = mapper;
            _claims = claims;
            _interviewService = interviewService;
        }

        public async Task<InterviewResponse> Add(IResolverContext context, [GraphQLName("interview")] AddInterviewRequest interviewRequest)
        {
            Status status = new();
            InterviewResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(interviewRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                var interview = _mapper.Map<InterviewModel>(interviewRequest);
                if (interview == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _interviewService.Add(interview);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<InterviewResponse>(interview);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }

        public async Task<InterviewResponse> Update(IResolverContext context, [GraphQLName("interview")] UpdateInterviewRequest interviewRequest)
        {
            Status status = new();
            InterviewModel interview = null;
            InterviewResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(interviewRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, interview) = await _interviewService.GetById(interviewRequest.Id);
                if (!status.IsSuccess)
                {
                    status.ErrorCode = ErrorCode.InterviewNull;
                    break;
                }

                interview = _mapper.Map(interviewRequest, interview);
                if (interview == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _interviewService.Update(interview);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<InterviewResponse>(interview);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }

        public async Task<InterviewResponse> Delete(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _interviewService.Delete(id);
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
    }
}
