using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using JB.Organization.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JB.Organization.Models;
using System.Linq.Expressions;

using JB.Organization.DTOs.Review.Responses;
using JB.Organization.DTOs.Review.Requests;
using JB.Organization.Models.Review;
using JB.Infrastructure.Models;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models.Authentication;

namespace JB.Organization.GraphQL.Review
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class ReviewQuery
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly IUserClaimsModel _claims;
        public ReviewQuery(
            IReviewService reviewService,
            IMapper mapper,
            IUserClaimsModel claims)
        {
            _claims = claims;
            _mapper = mapper;
            _reviewService = reviewService;
        }

        [GraphQLName("reviews")]
        public async Task<ReviewOverallResponse> Reviews(IResolverContext context, int? id, 
            ListReviewRequest filterRequest)
        {
            ReviewOverallResponse result = new();
            List<ReviewResponse> reviewResponses = new();
            List<ReviewRatingPercentageResponse> ratingResponses = new();
            List<ReviewModel> reviews = new();
            ReviewModel review = null;
            Status status = new();

            do
            {
                if (id > 0)
                {
                    (status, review) = await _reviewService.GetById(id.Value);
                    if (status.IsSuccess)
                    {
                        reviewResponses = new List<ReviewResponse>()
                        {
                            _mapper.Map<ReviewResponse>(review),
                        };

                       
                    }

                    break;
                }

                Expression<Func<ReviewModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<ReviewModel>();
                Expression<Func<ReviewModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.Id);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 1 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? false;

                (status, reviews) = await _reviewService.List(filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                reviewResponses = reviews.ConvertAll(x => _mapper.Map<ReviewResponse>(x));
            }
            while (false);
            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            if(reviewResponses != null)
            {
                var list = reviewResponses.Select(r => r.Rating).ToList();
                ratingResponses = CountOccurrences(list);

                result.ReviewResponses = reviewResponses;
                result.RatingPercentages = ratingResponses;
            }

            return result;
        }

        static private List<ReviewRatingPercentageResponse> CountOccurrences(List<float> list)
        {
            if (list == null || list.Count() < 1)
            {
                return null;
            }

            var dic = list.GroupBy(i => i);
            List<ReviewRatingPercentageResponse> res = new();
            int len = list.Count;
            foreach (var item in dic)
            {
                var percentage = (float) item.Count()*100 / len;
                res.Add( new ReviewRatingPercentageResponse
                {
                    Rating = item.Key,
                    Percentage = percentage
                });
            }
            return res;
        }
    }

   
}
