using JB.User.Models.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.User.DTOs.Profile
{
    public class UpdateUserProfileRequest
    {
        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public int? DefaultCVId { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Introduction { get; set; }

        public string Website { get; set; }

        public string Gender { get; set; }

        public DateTime? Birthdate { get; set; }

        public string Phone { get; set; }

        public string Github { get; set; }

        public string Reference { get; set; }

        public string[] Activities { get; set; }

        public string[] Certifications { get; set; }

        public string[] Awards { get; set; }

        public UserSkillModel[] Skills { get; set; }

        public UserEducationModel[] Educations { get; set; }

        public UserExperienceModel[] Experiences { get; set; }
        public int? ProfileStatus { get; set; }
    }
}
