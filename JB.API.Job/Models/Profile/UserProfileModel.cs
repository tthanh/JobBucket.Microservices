using JB.Job.Models.CV;
using JB.Job.Models.Organization;
using JB.Job.Models.User;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Job.Models.Profile
{
    public class UserProfileModel : IUserModel, IUserProfileModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int RoleId { get; set; }

        public string AvatarUrl { get; set; }

        public int? OrganizationId { get; set; }

        [NotMapped]
        public OrganizationModel Organization { get; set; }

        public int? DefaultCVId { get; set; }

        public string PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Introduction { get; set; }

        public string Website { get; set; }

        public string Gender { get; set; }

        public DateTime Birthdate { get; set; }

        public string Phone { get; set; }

        public string Github { get; set; }

        public string Reference { get; set; }

        public string[] Activities { get; set; }

        public string[] Certifications { get; set; }

        public string[] Awards { get; set; }
        
        [Column(TypeName = "jsonb")]
        public UserSkillModel[] Skills { get; set; }

        [Column(TypeName = "jsonb")]
        public UserEducationModel[] Educations { get; set; }

        [Column(TypeName = "jsonb")]
        public UserExperienceModel[] Experiences { get; set; }

        public int Views { get; set; }
    }
}
