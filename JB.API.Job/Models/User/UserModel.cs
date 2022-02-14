using JB.Job.Models.Organization;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Job.Models.User
{
    public class UserModel : IUserModel
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string AuthSource { get; set; }
        public int RoleId { get; set; }
        
        [NotMapped]
        public bool IsLockedOut { get; set; }

        [NotMapped]
        public string PasswordPlain { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public int? OrganizationId { get; set; }

        [NotMapped]
        public OrganizationModel Organization { get; set; }

        public int? DefaultCVId { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int Id { get; set; }
    }
}
