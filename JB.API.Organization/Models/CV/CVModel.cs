using JB.Organization.Models.User;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Organization.Models.CV
{
    public class CVModel : IUserProfileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CVName { get; set; }
        public string Title { get; set; }
        public string AvatarUrl { get; set; }
        public string Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Introduction { get; set; }
        public string Website { get; set; }
        public string Github { get; set; }
        public string Reference { get; set; }

        [Column(TypeName = "jsonb")]
        public UserSkillModel[] Skills { get; set; }
        public string[] Activities { get; set; }
        public string[] Certifications { get; set; }
        public string[] Awards { get; set; }
        public int UserId { get; set; }
        [NotMapped]
        public UserModel User { get; set; }
        public string CVTemplate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        [Column(TypeName = "jsonb")]
        public UserEducationModel[] Educations { get; set; }

        [Column(TypeName = "jsonb")]
        public UserExperienceModel[] Experiences { get; set; }
    }
}
