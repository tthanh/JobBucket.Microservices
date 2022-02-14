using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Review.Responses
{
    public class ReviewOverallResponse
    {
        public ICollection<ReviewResponse> ReviewResponses { get; set; }
        public ICollection<ReviewRatingPercentageResponse> RatingPercentages { get; set; }
    }
}
