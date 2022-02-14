using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JB.Authentication.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwtToken(UserClaimsModel claimModel, DateTime expire)
        {
            string tokenString = null;

            if (claimModel != null && !string.IsNullOrEmpty(claimModel.Email))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var role = EnumHelper.GetDescriptionFromEnumValue((RoleType)claimModel.RoleId);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.Email, claimModel.Email),
                            new Claim(ClaimTypes.NameIdentifier, claimModel.Id.ToString()),
                            new Claim(ClaimTypes.Role, role),
                    }),
                    Expires = expire,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                tokenString = tokenHandler.WriteToken(token);
            }

            return tokenString;
        }

        public bool ValidateToken(string token, out JwtSecurityToken jwtToken)
        {
            bool result = false;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            jwtToken = (JwtSecurityToken)validatedToken;
            result = true;

            return result;
        }
    }
}
