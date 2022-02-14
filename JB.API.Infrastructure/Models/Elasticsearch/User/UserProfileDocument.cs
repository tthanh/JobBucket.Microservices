using JB.Infrastructure.Models.Elasticsearch.User.Property;
using JB.Infrastructure.Elasticsearch.Organization;
using System;

namespace JB.Infrastructure.Elasticsearch.User
{
    public class UserProfileDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public string AvatarUrl { get; set; }
        public int? OrganizationId { get; set; }
        public OrganizationDocument Organization { get; set; }
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
        public UserSkillDocument[] Skills { get; set; }
        public UserEducationDocument[] Educations { get; set; }
        public UserExprerienceDocument[] Experiences { get; set; }
        public int Views { get; set; }
    }
}
