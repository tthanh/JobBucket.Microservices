using JB.Authentication.Data;
using JB.Authentication.Models.User;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthenticationDbContext _authenticationdbContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            AuthenticationDbContext authenticationdbContext,
            UserManager<UserModel> userManager,
            IEmailService emailService,
            ILogger<AuthenticationService> logger
            )
        {
            _authenticationdbContext = authenticationdbContext;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticate a user by username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<(Status, UserModel)> Authenticate(string username, string password)
        {
            Status result = new();
            UserModel user = null;

            do
            {
                try
                {
                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                        break;

                    var identityUser = await _authenticationdbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
                    if (identityUser == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    var role = (await _userManager.GetRolesAsync(identityUser))?.FirstOrDefault();
                    if (!Role.FromString.TryGetValue(role, out var roleType))
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    identityUser.RoleId = (int)roleType;

                    //Check if user is locked
                    bool isLocked = await IsUserLocked(identityUser);
                    if (isLocked)
                    {
                        result.ErrorCode = ErrorCode.AccountLocked;
                        break;
                    }

                    //Check user email validated
                    if (!identityUser.EmailConfirmed)
                    {
                        result.ErrorCode = ErrorCode.EmailNotConfirmed;
                        break;
                    }

                    //Check user password
                    bool isPasswordMatch = await _userManager.CheckPasswordAsync(identityUser, password);
                    if (!isPasswordMatch)
                    {
                        result.ErrorCode = ErrorCode.InvalidPassword;
                        break;
                    }

                    user = identityUser;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                    break;
                }
            }
            while (false);

            return (result, user);
        }

        public async Task<Status> Register(UserModel user)
        {
            Status result = new Status();
            IdentityResult identityResult;
            int roleId = 0;

            do
            {
                try
                {
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    var userInDb = await _userManager.FindByNameAsync(user.Email);

                    //Already exist
                    if (userInDb != null)
                    {
                        result.ErrorCode = ErrorCode.EmailAlreadyRegistered;
                        break;
                    }

                    if (user.RoleId != (int)RoleType.User && user.RoleId != (int)RoleType.Recruiter && user.RoleId != (int)RoleType.OrganizationManager)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Create user
                    identityResult = await _userManager.CreateAsync(user, user.PasswordPlain);
                    if (!identityResult.Succeeded)
                    {
                        result.SetStatusMessage(ErrorCode.InvalidData, string.Join(',', identityResult.Errors.Select(x => x.Description)));
                    }

                    identityResult = await _userManager.AddToRoleAsync(user, EnumHelper.GetDescriptionFromEnumValue((RoleType)user.RoleId));
                    if (!identityResult.Succeeded)
                    {
                        result.SetStatusMessage(ErrorCode.InvalidData, string.Join(',', identityResult.Errors.Select(x => x.Description)));
                    }

                    //Send email
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    _emailService.SendEmailAsync(user.Email, "[JobBucket] - Confirm your email",
                    $"Your confirmation code is: {code}");
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.InvalidData;
                    _logger.LogError(e, e.Message);
                    break;
                }

            }
            while (false);

            return result;
        }

        public async Task<Status> ConfirmEmail(string email, string code)
        {
            Status result = new Status();
            UserModel user;
            IdentityResult identityResult;

            do
            {
                try
                {
                    //Check argument
                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    //Check if email confirmed
                    if (user.EmailConfirmed)
                    {
                        result.ErrorCode = ErrorCode.EmailAlreadyConfirmed;
                        break;
                    }

                    //Confirm email with code
                    identityResult = await _userManager.ConfirmEmailAsync(user, code);
                    if (!identityResult.Succeeded)
                    {
                        result.SetStatusMessage(ErrorCode.InvalidVerifyCode, string.Join(',', identityResult.Errors.Select(x => x.Description)));
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.InvalidData;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return result;
        }

        public async Task<Status> ResendConfirmationEmail(string username)
        {
            Status result = new Status();
            UserModel user;
            string code = string.Empty;

            do
            {
                try
                {
                    //Check argument
                    if (string.IsNullOrEmpty(username))
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _authenticationdbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    //Send email
                    code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    _emailService.SendEmailAsync(user.Email, "[JobBucket] - Confirm your email",
                    $"Your confirmation code is: {code}");
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.InvalidData;
                    _logger.LogError(e, e.Message);
                    break;
                }

            }
            while (false);

            return result;
        }

        public async Task<Status> ResetPassword(string email)
        {
            Status result = new Status();
            UserModel user;
            string resetToken;

            do
            {
                try
                {
                    //Check argument
                    if (string.IsNullOrEmpty(email))
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    //Check if email confirmed
                    if (!user.EmailConfirmed)
                    {
                        result.ErrorCode = ErrorCode.EmailNotConfirmed;
                        break;
                    }

                    //Generate token
                    resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    if (string.IsNullOrEmpty(resetToken))
                    {
                        result.ErrorCode = ErrorCode.ServerError;
                        break;
                    }

                    //Send email
                    _emailService.SendEmailAsync(user.Email, "[JobBucket] - Reset Your Password",
                    $"Your reset password code is: {resetToken}");
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return result;
        }

        public async Task<Status> ConfirmResetPassword(string email, string resetToken, string newPassword)
        {
            Status result = new Status();
            UserModel user;
            IdentityResult identityResult;

            do
            {
                try
                {
                    //Check argument
                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(resetToken))
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    //Check if email confirmed
                    if (!user.EmailConfirmed)
                    {
                        result.ErrorCode = ErrorCode.EmailNotConfirmed;
                        break;
                    }

                    //Reset password with code
                    identityResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                    if (!identityResult.Succeeded)
                    {
                        result.SetStatusMessage(ErrorCode.InvalidVerifyCode, string.Join(',', identityResult.Errors.Select(x => x.Description)));
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.InvalidData;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return result;
        }

        /// <summary>
        /// Check if user is locked
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> IsUserLocked(UserModel user)
        {
            if (user == null)
                return false;

            DateTimeOffset? lockUntil = await _userManager.GetLockoutEndDateAsync(user);
            if (lockUntil == null)
                return false;

            if (DateTime.Compare(lockUntil.Value.DateTime.ToUniversalTime(), DateTime.UtcNow) > 0)
                return true;

            return false;
        }

    }
}
