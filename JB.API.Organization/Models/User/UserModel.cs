using JB.Organization.Models.Organization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace JB.Organization.Models.User
{
    public class UserModel : IdentityUser<int>, IUserModel
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
        
    }
}
