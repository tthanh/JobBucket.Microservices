using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Organization
{
    public class AddOrganizationRequest
    {
        public string Name { get; set; }

        public string Bio { get; set; }

        public string Country { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string[] Addresses { get; set; }

        public string[] ImageUrls { get; set; }

        public string AvatarUrl { get; set; }

       
    }
}
