using System.ComponentModel.DataAnnotations;

namespace JB.Authentication.DTOs.Authentication
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
