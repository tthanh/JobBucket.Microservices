using JB.Lib.Models.User;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace JB.Authentication.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(UserClaimsModel UserModel, DateTime expire);
        bool ValidateToken(string token, out JwtSecurityToken jwtToken);
    }
}
