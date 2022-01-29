using JB.Infrastructure.Models.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace JB.Gateway.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(UserClaimsModel UserModel, DateTime expire);
        bool ValidateToken(string token, out JwtSecurityToken jwtToken);
    }
}
