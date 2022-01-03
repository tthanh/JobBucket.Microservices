using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Review.Requests
{
    public class UpdateReviewRequest
    {
        [Required]
        public int Id { get; set; }
        public float Rating { get; set; }
        public float RatingBenefit { get; set; }
        public float RatingLearning { get; set; }
        public float RatingCulture { get; set; }
        public float RatingWorkspace { get; set; }
        public string Content { get; set; }
       
       
    }
}
