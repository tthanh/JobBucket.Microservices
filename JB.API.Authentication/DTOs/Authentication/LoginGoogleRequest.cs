using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Authentication.DTOs.Authentication
{
    public class LoginGoogleRequest
    {
        [Required]
        public string GoogleId { get; set; }

        [Required]
        public string TokenId { get; set; }

        [Range(1, 3)]
        public int RoleId { get; set; } = 1;
    }
}
