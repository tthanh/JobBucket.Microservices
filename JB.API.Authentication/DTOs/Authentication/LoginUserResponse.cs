using JB.Authentication.Models;
using System;

namespace JB.Authentication.DTOs.Authentication
{
    public class LoginUserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LockoutEnd { get; set; }
    }
}
