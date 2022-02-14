using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Authentication.DTOs.UserManagement
{
    public class GetUserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string[] AddressLine { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsLockedOut { get; set; }
        public string FullName { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public int RoleId { get; set; }
    }
}
