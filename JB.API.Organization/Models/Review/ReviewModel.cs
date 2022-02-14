using JB.Organization.Models.Organization;
using JB.Organization.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using JB.Infrastructure.Models;

namespace JB.Organization.Models.Review
{
    public class ReviewModel : IEntityDate
    {
        public int Id { get; set; }
        public float Rating { get; set; } = 0;
        public float RatingBenefit { get; set; } = 0;
        public float RatingLearning { get; set; } = 0;
        public float RatingCulture { get; set; } = 0;
        public float RatingWorkspace { get; set; } = 0;
        public string Content { get; set; }
        public int OrganizationId { get; set; }
        [NotMapped]
        public OrganizationModel Organization { get; set; }
        public virtual ICollection<ReviewInterestModel> Interests { get; set; }
        [NotMapped]
        public int InterestCount { get; set; }
        [NotMapped]
        public UserModel User { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        [NotMapped]
        public bool IsInterested { get; set; }
    }
}
