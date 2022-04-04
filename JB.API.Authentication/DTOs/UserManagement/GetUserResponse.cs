using JB.gRPC.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Authentication.DTOs.UserManagement
{
    public class GetUserResponse
    {
        public string Name { get; set; }
        public int RoleId { get; set; }
        public string AvatarUrl { get; set; }
        public int? OrganizationId { get; set; }
        public OrganizationResponse Organization { get; set; }
        public int? DefaultCVId { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int Id { get; set; }
        public bool IsLockedOut { get; set; }
        public int ProfileStatus { get; set; }
    }
}
