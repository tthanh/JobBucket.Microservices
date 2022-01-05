using JB.Authentication.Models;
using System;

namespace JB.Authentication.DTOs.Authentication
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public LoginUserResponse User { get; set; }
    }
}
