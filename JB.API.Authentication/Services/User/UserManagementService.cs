using JB.Authentication.Data;
using JB.Authentication.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.Infrastructure.Models;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;

namespace JB.Authentication.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly AuthenticationDbContext _authenticationdbContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IDistributedCache _cache;

        public UserManagementService(
            AuthenticationDbContext authenticationdbContext,
            UserManager<UserModel> userManager,
            ILogger<AuthenticationService> logger,
            IDistributedCache cache
            )
        {
            _authenticationdbContext = authenticationdbContext;
            _userManager = userManager;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Status> LockUser(int userId, DateTime lockUntil)
        {
            Status result = new Status();
            UserModel user;
            IdentityResult identityResult;
            bool canBeLocked;

            do
            {
                try
                {
                    //Check argument
                    if (userId < 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _authenticationdbContext.Users.FirstAsync(x => x.Id == userId);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    //Check if user can be locked
                    canBeLocked = await _userManager.GetLockoutEnabledAsync(user);
                    if (!canBeLocked)
                    {
                        result.ErrorCode = ErrorCode.AccountCanNotBeLocked;
                        break;
                    }

                    //Set user locked
                    identityResult = await _userManager.SetLockoutEndDateAsync(user, lockUntil);
                    if (!identityResult.Succeeded)
                    {
                        result.SetStatusMessage(ErrorCode.InvalidVerifyCode, string.Join(',', identityResult.Errors.Select(x => x.Description)));
                    }
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

        public async Task<Status> UnlockUser(int userId)
        {
            Status result = new Status();
            UserModel user;
            IdentityResult identityResult;
            bool isUserLocked;

            do
            {
                try
                {
                    //Check argument
                    if (userId < 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _authenticationdbContext.Users.FirstAsync(x => x.Id == userId);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    //Check if user is locked
                    isUserLocked = await _userManager.GetLockoutEnabledAsync(user);
                    if (!isUserLocked)
                    {
                        result.ErrorCode = ErrorCode.AccountNotLocked;
                        break;
                    }

                    //Set user unlocked locked
                    identityResult = await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(-3));
                    if (!identityResult.Succeeded)
                    {
                        result.SetStatusMessage(ErrorCode.InvalidVerifyCode, string.Join(',', identityResult.Errors.Select(x => x.Description)));
                    }
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

        public async Task<(Status, List<UserModel>)> ListUser(Expression<Func<UserModel, bool>> filters, Expression<Func<UserModel, object>> sorts, int size, int offset)
        {
            Status result = new();
            List<UserModel> users = null;

            do
            {
                try
                {
                    if (size < 0 || offset < 1)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    //Find user
                    users = await _userManager.Users
                        .Where(filters)
                        .OrderBy(sorts)
                        .Skip((offset - 1) * size)
                        .Take(size).ToListAsync();

                    if (users == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    foreach (var user in users)
                    {
                        user.IsLockedOut = await _userManager.IsLockedOutAsync(user);
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return (result, users);
        }

        public async Task<(Status, UserModel)> GetUser(int userId)
        {
            Status result = new Status();
            UserModel user = null;
            bool isSetCache = false;
            do
            {
                try
                {
                    //Check argument
                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    user = await _cache.GetAsync<UserModel>($"user-{userId}");
                    if (user != null)
                    {
                        break;
                    }
                    else
                    {
                        isSetCache = true;
                    }


                    //Find user
                    user = await _authenticationdbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    //user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    user.IsLockedOut = await _userManager.IsLockedOutAsync(user);

                    if (isSetCache)
                    {
                        await _cache.SetAsync<UserModel>($"user-{userId}", user, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                            SlidingExpiration = TimeSpan.FromHours(1),
                        });
                    }

                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return (result, user);
        }

        public async Task<(Status, UserModel)> GetUser(string userName, string authSource = null)
        {
            Status result = new Status();
            UserModel user = null;

            do
            {
                try
                {
                    //Check argument
                    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(authSource))
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _authenticationdbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.AuthSource == authSource);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return (result, user);
        }

        #region organization
        public async Task<(Status, UserModel)> PromoteTo(int userId, int organizationId, int roleId)
        {
            Status result = new Status();
            UserModel user = null;
            do
            {
                try
                {
                    //Check argument
                    if (organizationId <= 0 || userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _authenticationdbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    //user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    if (user.OrganizationId == null || user.OrganizationId <= 0)
                    {
                        user.OrganizationId = organizationId;
                    }
                    user.RoleId = roleId;
                    _authenticationdbContext.Update(user);
                    await _authenticationdbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return (result, user);
        }

        public async Task<(Status, List<UserModel>)> DemoteToUser(int organizationId)
        {
            Status result = new Status();
            List<UserModel> users = null;
            do
            {
                try
                {
                    //Check argument
                    if (organizationId <= 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    users = await _authenticationdbContext.Users.Where(u => u.OrganizationId == organizationId).ToListAsync();
                    //user = await _userManager.FindByIdAsync(userId);
                    if (users == null || users.Count() == 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    foreach(var user in users)
                    {
                        user.OrganizationId = 0;
                        user.RoleId = (int)RoleType.User;
                        _authenticationdbContext.Update(user);
                    }
                    await _authenticationdbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return (result, users);
        }
        #endregion

        #region CV
        public async Task<(Status, UserModel)> UpdateUserDefaultCV(int userId, int cvId)
        {
            Status result = new Status();
            UserModel user = null;
            do
            {
                try
                {
                    //Check argument
                    if (cvId <= 0 || userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _authenticationdbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    //user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    user.DefaultCVId = cvId;
                    _authenticationdbContext.Update(user);
                    await _authenticationdbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return (result, user);
        }
        public async Task<(Status, UserModel)> DeleteUserDefaultCV(int userId)
        {
            Status result = new Status();
            UserModel user = null;
            do
            {
                try
                {
                    //Check argument
                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    //Find user
                    user = await _authenticationdbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    //user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    user.DefaultCVId = null;
                    _authenticationdbContext.Update(user);
                    await _authenticationdbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return (result, user);
        }
        #endregion

        public async Task<(Status, int)> CountUser(Expression<Func<UserModel, bool>> filters)
        {
            Status result = new Status();
            int usersCount = 0;
            do
            {
                try
                {
                    if (filters == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    //Find user
                    usersCount = await _userManager.Users.Where(filters)?.CountAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return (result, usersCount);
        }

        public async Task<(Status, List<UserModel>)> GetUsers(List<int> userIds)
        {
            Status result = new Status();
            List<UserModel> users = new List<UserModel>();
            List<int> notCachedIds = new List<int>();
            do
            {
                try
                {
                    //Check argument
                    if (userIds == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    foreach (var id in userIds)
                    {
                        var user = await _cache.GetAsync<UserModel>($"user-{id}");
                        if (user == null)
                        {
                            notCachedIds.Add(id);
                            continue;
                        }

                        users.Add(user);
                    }

                    if (notCachedIds.Count > 0)
                    {
                        //Find user
                        var usersFromDb = await _authenticationdbContext.Users
                            .Where(u => notCachedIds.Contains(u.Id))
                            .ToListAsync();

                        foreach (var u in usersFromDb)
                        {
                            await _cache.SetAsync<UserModel>($"user-{u.Id}", u, new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                                SlidingExpiration = TimeSpan.FromHours(1),
                            });
                        }

                        users.AddRange(usersFromDb);
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

            return (result, users);
        }

        public async Task<(Status, List<int>)> GetUserIds(Expression<Func<UserModel, bool>> filters)
        {
            Status result = new();
            List<int> userIds = null;

            do
            {
                try
                {
                    //List user ids
                    userIds = await _userManager.Users.Where(filters).Select(u => u.Id).ToListAsync();

                    if (userIds == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            while (false);

            return (result, userIds);
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

        public async Task<(Status, UserModel)> CreateUser(UserModel user)
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

                    var userInDb = await _userManager.FindByNameAsync(user.UserName);

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

                    user.EmailConfirmed = true;

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
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.InvalidData;
                    _logger.LogError(e, e.Message);
                    break;
                }

            }
            while (false);

            return (result,user);
        }

        public async Task<Status> DeleteUser(UserModel user)
        {
            Status result = new();

            do
            {
                try
                {
                    //Check argument
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    _authenticationdbContext.Users.Remove(user);
                    await _authenticationdbContext.SaveChangesAsync();
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

        public async Task<Status> DeleteUser(int userId)
        {
            Status result = new();

            do
            {
                try
                {
                    //Check argument
                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    //Find user
                    var user = await _authenticationdbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    _authenticationdbContext.Users.Remove(user);
                    await _authenticationdbContext.SaveChangesAsync();
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
        
        public async Task<(Status, UserModel)> UpdateUser(UserModel user)
        {
            Status result = new Status();
            UserModel userFromDb = null;
            do
            {
                if (user == null || user.Id <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }

                try
                {
                    userFromDb = await _authenticationdbContext.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
                    if (userFromDb == null)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    PropertyHelper.InjectNonNull<UserModel>(userFromDb, user);
                    _authenticationdbContext.Update(userFromDb);
                    await _authenticationdbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, userFromDb);
        }
    }
}
