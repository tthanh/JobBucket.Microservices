using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JB.Authentication.DTOs.Authentication
{
    public class RegisterRequest
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Required]
        [Range(1, 3)]
        public int RoleId { get; set; }
    }
}
