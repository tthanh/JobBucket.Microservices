using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Review.Requests
{
    public class AddReviewRequest
    {
        public float Rating { get; set; } 
        public float RatingBenefit { get; set; } 
        public float RatingLearning { get; set; }
        public float RatingCulture { get; set; }
        public float RatingWorkspace { get; set; }
        public string Content { get; set; }
        public int OrganizationId { get; set; }
      
       
      
    }
}
