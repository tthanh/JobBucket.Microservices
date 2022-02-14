using JB.Blog.Models.User;
using JB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Blog.Models.Organization
{
    public class OrganizationModel : IEntityDate
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

        public int[] ManagerIds { get; set; }

        [NotMapped]
        public ICollection<UserModel> Managers { get; set; }

        public int[] EmployerIds { get; set; }

        [NotMapped]
        public ICollection<UserModel> Employers { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public float Rating { get; set; } = 0;

        public float RatingBenefit { get; set; } = 0;

        public float RatingLearning { get; set; } = 0;

        public float RatingCulture { get; set; } = 0;

        public float RatingWorkspace { get; set; } = 0;
    }
}
