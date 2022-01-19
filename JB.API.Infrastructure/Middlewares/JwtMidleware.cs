using JB.Infrastructure.Constants;
using JB.Infrastructure.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.API.Infrastructure.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IUserClaimsModel userClaims)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;

            if (token.StartsWith("Bearer"))
            {
                token = token[6..]?.Trim();
            }

            if (string.IsNullOrEmpty(token))
            {
                await _next(context);
                return;
            }

            JwtSecurityToken jwtSecurityToken = null;

            try
            {
                _ = ValidateToken(token, out jwtSecurityToken);
            }
            catch (Exception ex)
            {
                if (ex is SecurityTokenExpiredException)
                {
                    // If token expired, return 401
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(
                        new
                        {
                            message = "Token expired"
                        }
                    );
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(
                        new
                        {
                            message = "Token is invalid"
                        }
                    );
                return;
            }

            (bool isValid, int userId, string email, int roleId) = ExtractTokenInformation(jwtSecurityToken);
            if (!isValid)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(
                        new
                        {
                            message = "Token is invalid"
                        }
                    );
                return;
            }

            userClaims.Id = userId;
            userClaims.Email = email;
            userClaims.RoleId = roleId;

            await _next(context);
        }

        private (bool IsValid, int UserId, string Email, int RoleId) ExtractTokenInformation(JwtSecurityToken jwtToken)
        {
            bool isValid = false;
            int userId = -1;
            string email = null;
            int roleId = -1;

            do
            {
                if (!int.TryParse(jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value, out userId))
                {
                    break;
                }

                email = jwtToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    break;
                }

                var role = jwtToken.Claims.FirstOrDefault(x => x.Type == "role")?.Value;
                if (string.IsNullOrEmpty(role))
                {
                    break;
                }

                if (!Role.FromString.TryGetValue(role, out RoleType roleType))
                {
                    break;
                }
                roleId = (int)roleType;

                isValid = true;
            }
            while (false);

            return (isValid, userId, email, roleId);
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
