using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Organization
{
    public class OrganizationUserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public int OrganizationId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int RoleId { get; set; }
    }
}
