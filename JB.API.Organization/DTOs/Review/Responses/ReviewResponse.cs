using JB.Organization.Models.Organization;
using JB.Organization.Models.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Review.Responses
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public float Rating { get; set; }
        public float RatingBenefit { get; set; }
        public float RatingLearning { get; set; }
        public float RatingCulture { get; set; } 
        public float RatingWorkspace { get; set; }
        public string Content { get; set; }
        public int OrganizationId { get; set; }
        
        public ReviewOrganizationResponse Organization { get; set; }
        public virtual ICollection<ReviewInterestResponse> Interests { get; set; }
        public int InterestCount { get; set; }
        public ReviewUserResponse User { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsInterested { get; set; }
    }
}
