using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Organization
{
    public class AddEmployerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }

        public string PasswordPlain { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int RoleId { get; set; }


        public string AvatarUrl { get; set; }

    }
}
