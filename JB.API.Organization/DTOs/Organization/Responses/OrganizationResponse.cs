using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Organization
{
    public class OrganizationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string[] Addresses { get; set; }
        public string[] ImageUrls { get; set; }
        public string AvatarUrl { get; set; }
        public float Rating { get; set; }
        public float RatingBenefit { get; set; }
        public float RatingLearning { get; set; }
        public float RatingCulture { get; set; }
        public float RatingWorkspace { get; set; }
        public bool IsReviewAllowed { get; set; }
    }
}
