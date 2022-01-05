using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using JB.Lib.Models.User;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using JB.Authentication.DTOs.Authentication;
using JB.Authentication.Services;
using JB.Authentication.Models.User;
using Swashbuckle.AspNetCore.Annotations;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;

namespace JB.Authentication.User.Controllers
{
    /// <summary>
    /// Authenticate service
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationService _authService;
        private readonly IUserManagementService _userManagementService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IUserClaimsModel _claims;

        public AuthenticateController(
            IConfiguration configuration,
            IJwtService jwtService,
            IMapper mapper,
            IUserClaimsModel claims,
            IAuthenticationService authService,
            IUserManagementService userManagementService
            )
        {
            _configuration = configuration;
            _jwtService = jwtService;
            _mapper = mapper;
            _claims = claims;
            _authService = authService;
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Login using username and password
        /// </summary>
        /// <param name="model">Login request model</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var canParseAccessTokenMinute = int.TryParse(_configuration.GetSection("Jwt:AccessTokenMinute")?.Value, out int accessTokenMinute);
            var canParseRefreshTokenMinute = int.TryParse(_configuration.GetSection("Jwt:RefreshTokenMinute")?.Value, out int refreshTokenMinute);

            if (!canParseAccessTokenMinute || !canParseRefreshTokenMinute)
            {
                return StatusCode(500);
            }

            (var status, var user) = await _authService.Authenticate(model.Username, model.Password);
            if (!status.IsSuccess)
            {
                var statusCode = 404;

                switch (status.ErrorCode)
                {
                    case ErrorCode.EmailNotConfirmed:
                        statusCode = 401;
                        break;
                }

                return StatusCode(statusCode, new { status.Message });
            }

            var claimsModel = _mapper.Map<UserClaimsModel>(user);
            var accessToken = _jwtService.GenerateJwtToken(claimsModel, DateTime.UtcNow.AddMinutes(accessTokenMinute));
            var refreshToken = _jwtService.GenerateJwtToken(claimsModel, DateTime.UtcNow.AddMinutes(refreshTokenMinute));

            return Ok(new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = _mapper.Map<LoginUserResponse>(user),
            });
        }

        /// <summary>
        /// Login using Google account
        /// </summary>
        /// <param name="googleLoginVM">Google Login request</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoginGoogle(LoginGoogleRequest googleLoginRequest)
        {
            var canParseAccessTokenMinute = int.TryParse(_configuration.GetSection("Jwt:AccessTokenMinute")?.Value, out int accessTokenMinute);
            var canParseRefreshTokenMinute = int.TryParse(_configuration.GetSection("Jwt:RefreshTokenMinute")?.Value, out int refreshTokenMinute);

            var payload = await ValidateAsync(googleLoginRequest.TokenId, new ValidationSettings
            {
                Audience = new[] { _configuration["ExternalAuth:Google:ClientId"] }
            });

            if (payload == null)
            {
                return BadRequest();
            }

            string email = payload.Email;
            string name = payload.Name;
            string avatarUrl = payload.Picture;
            string id = googleLoginRequest.GoogleId;

            (var result, var user) = await _userManagementService.GetUser(id, "Google");
            if (user == null)
            {
                user = new UserModel
                {
                    Name = name,
                    Email = email,
                    UserName = id,
                    AuthSource = "Google",
                    RoleId = googleLoginRequest.RoleId,
                    EmailConfirmed = true,
                    AvatarUrl = avatarUrl
                };

                (result, user) = await _userManagementService.CreateUser(user);
                if (!result.IsSuccess)
                {
                    return BadRequest();
                }
            }

            var claimsModel = _mapper.Map<UserClaimsModel>(user);

            var accessToken = _jwtService.GenerateJwtToken(claimsModel, DateTime.UtcNow.AddMinutes(accessTokenMinute));
            var refreshToken = _jwtService.GenerateJwtToken(claimsModel, DateTime.UtcNow.AddMinutes(refreshTokenMinute));

            return Ok(new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = _mapper.Map<LoginUserResponse>(user),
            });
        }

        /// <summary>
        /// Register a new account
        /// </summary>
        /// <param name="model">Register request model</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            UserModel user = _mapper.Map<UserModel>(model);

            var status = await _authService.Register(user);
            if (!status.IsSuccess)
            {
                return StatusCode(404, new { status.Message });
            }

            return Ok();
        }

        /// <summary>
        /// Confirm account using code sent to email
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="code">Received code</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([FromQuery][Required] string email, [Required][FromQuery] string code)
        {
            var status = await _authService.ConfirmEmail(email, code);
            if (!status.IsSuccess)
            {
                return StatusCode(404, new { status.Message });
            }

            return Ok();
        }

        /// <summary>
        /// Resend confirmation email if email not sent
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ResendConfirmationEmail([FromQuery][Required] string email)
        {
            var status = await _authService.ResendConfirmationEmail(email);
            if (!status.IsSuccess)
            {
                return StatusCode(404, new { status.Message });
            }

            return Ok();
        }

        /// <summary>
        /// Confirm reset password with code sent to email
        /// </summary>
        /// <param name="model">Reset password request</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] ResetPasswordRequest model)
        {
            var status = await _authService.ConfirmResetPassword(model.Email, model.ResetToken, model.Password);
            if (!status.IsSuccess)
            {
                return StatusCode(404, new { status.Message });
            }

            return Ok();
        }

        /// <summary>
        /// Request a password reset, code will be sent to email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ResetPassword([FromQuery] string email)
        {
            var status = await _authService.ResetPassword(email);
            if (!status.IsSuccess)
            {
                return StatusCode(404, new { status.Message });
            }

            return Ok();
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="refreshTokenVM"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenVM)
        {
            var status = new Status();
            bool isValidToken;
            int userId;
            UserModel user = null;
            string refreshToken = null;

            refreshToken = refreshTokenVM.RefreshToken;
            if (string.IsNullOrEmpty(refreshToken))
            {
                return StatusCode(403, new { status.Message });
            }

            isValidToken = _jwtService.ValidateToken(refreshToken, out var jwtToken);
            if (!isValidToken)
            {
                return StatusCode(403, new { status.Message });
            }

            var canParseUserId = int.TryParse(jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value, out userId);
            if (!canParseUserId || userId <= 0)
            {
                return StatusCode(403, new { status.Message });
            }

            (status, user) = await _userManagementService.GetUser(userId);
            if (!status.IsSuccess)
            {
                return StatusCode(404, new { status.Message });
            }

            if (!int.TryParse(_configuration.GetSection("Jwt:AccessTokenMinute")?.Value, out int accessTokenMinute))
            {
                return StatusCode(503, new { status.Message });
            }

            var claimsModel = _mapper.Map<UserClaimsModel>(user);
            string newAccessToken = _jwtService.GenerateJwtToken(claimsModel, DateTime.UtcNow.AddMinutes(accessTokenMinute));
            if (string.IsNullOrEmpty(newAccessToken))
            {
                return StatusCode(404, new { status.Message });
            }

            return Ok(new
            {
                accessToken = newAccessToken,
            });
        }

        private static string ToCamelCase(string titleCase)
        {
            return char.ToLowerInvariant(titleCase[0]) + titleCase.Substring(1);
        }
    }
}