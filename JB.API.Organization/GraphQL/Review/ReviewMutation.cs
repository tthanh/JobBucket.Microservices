using JB.Organization.Services;
using System.Threading.Tasks;
using HotChocolate.Resolvers;
using AutoMapper;
using HotChocolate;
using JB.Organization.DTOs.Review.Responses;
using JB.Organization.DTOs.Review.Requests;
using JB.Organization.Models.Review;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;

namespace JB.Organization.GraphQL.Review
{
    public class ReviewMutation
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly IUserClaimsModel _claims;
        
        public ReviewMutation(
            IReviewService reviewService,
            IMapper mapper,
            IUserClaimsModel claims
            )
        {
            _claims = claims;
            _mapper = mapper;
            _reviewService = reviewService;
        }

        public async Task<ReviewResponse> Add(IResolverContext context,
            [GraphQLName("review")] AddReviewRequest reviewRequest)
        {
            Status status = new();
            ReviewResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(reviewRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                var review = _mapper.Map<ReviewModel>(reviewRequest);
                if (review == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _reviewService.Add(review);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<ReviewResponse>(review);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<ReviewResponse> Update(IResolverContext context,
            [GraphQLName("review")] UpdateReviewRequest reviewRequest)
        {
            Status status = new();
            ReviewModel review = null;
            ReviewResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(reviewRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, review) = await _reviewService.GetById(reviewRequest.Id);
                if (!status.IsSuccess)
                {
                    break;
                }

                review = _mapper.Map(reviewRequest, review);
                if (review == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _reviewService.Update(review);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<ReviewResponse>(review);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<ReviewResponse> Delete(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _reviewService.Delete(id);
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
        public async Task<ReviewResponse> Interest(IResolverContext context, 
            int id)
        { 
            Status status = new();
            ReviewResponse result = null;
            ReviewModel model = null;

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, model) = await _reviewService.Interest(id);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<ReviewResponse>(model);
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
