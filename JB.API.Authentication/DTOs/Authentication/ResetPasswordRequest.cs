using System.ComponentModel.DataAnnotations;

namespace JB.Authentication.DTOs.Authentication
{
    public class ResetPasswordRequest
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetToken { get; set; }
    }
}
