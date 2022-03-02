using AutoMapper;
using JB.API.User.Constants;
using JB.gRPC.Organization;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.User.Data;
using JB.User.Models.CV;
using JB.User.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.User.Services
{
    public class CVService : ICVService
    {
        private readonly CVDbContext _cvDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CVService> _logger;
        private readonly IUserClaimsModel _claims;

        private readonly IUserManagementService _userService;
        private readonly IDistributedCache _cache;

        public CVService(
           CVDbContext cvDbContext,
           IMapper mapper,
           ILogger<CVService> logger,
           IUserClaimsModel claims,
           IUserManagementService userService,
           IDistributedCache cache
       )
        {
            _cvDbContext = cvDbContext;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _userService = userService;
            _cache = cache;
        }

       
        //Will be update after payment system complete.
        //..
        private int MaxCVNum(int UserId)
        {
            return CVMaxNum.NORMAL_MAX_CV;
        }

        private async Task<int> CountCV(int UserId)
        {
            int count = 0;
            count = await _cvDbContext.CVs.Where(c => c.UserId == UserId).CountAsync();
            return count;
        }


        public async Task<Status> Add(CVModel model)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;

            do
            {
                if (model == null)
                {
                    result.ErrorCode = ErrorCode.cvNull;
                    break;
                }

                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.NoPrivilege;
                    break;
                }

                int count = await CountCV(userId);
                int max = MaxCVNum(userId);
                if(count >= max)
                {
                    result.ErrorCode = ErrorCode.cvMax;
                    break;
                }

                model.UserId = userId;
                try
                {
                    await _cvDbContext.CVs.AddAsync(model);
                    await _cvDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, long)> Count(Expression<Func<CVModel, bool>> predicate)
        {
            Status result = new Status();
            long cvCount = 0;
            do
            {
                if (predicate == null)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
                try
                {
                    cvCount = await _cvDbContext.CVs.Where(predicate).CountAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, cvCount);
        }

        public async Task<Status> Delete(int cvId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;

            do
            {
                if (cvId <= 0)
                {
                    result.ErrorCode = ErrorCode.cvNull;
                    break;
                }

                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.NoPrivilege;
                    break;
                }

                try
                {
                    var cvModel = await _cvDbContext.CVs.FirstOrDefaultAsync(x => x.Id == cvId);
                    if (cvModel == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }
                    if(cvModel.UserId != userId)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _cvDbContext.CVs.Remove(cvModel);
                    await _cvDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, CVModel)> GetById(int cvId)
        {
            Status result = new Status();
            CVModel cv = null;
            bool isSetCache = false;

            do
            {
                try
                {
                    cv = await _cache.GetAsync<CVModel>($"cv-{cvId}");
                    if (cv != null)
                    {
                        break;
                    }
                    else
                    {
                        isSetCache = true;
                    }

                    cv = await _cvDbContext.CVs.Where(x => x.Id == cvId).FirstOrDefaultAsync();
                    if (cv == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    if (cv.UserId > 0)
                    {
                        UserModel user = _userService.GetUser(cv.UserId).Result.Item2;
                        if (user != null)
                        {
                            cv.User = user;
                        }
                    }

                    if (isSetCache)
{
                        await _cache.SetAsync<CVModel>($"cv-{cvId}", cv, new DistributedCacheEntryOptions
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
                }

            }
            while (false);

            return (result, cv);
        }

        public async Task<(Status, List<CVModel>)> List(Expression<Func<CVModel, bool>> filter, Expression<Func<CVModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var cvs = new List<CVModel>();
            int userId = _claims?.Id ?? 0;
            do
            {
                try
                {
                    var cvQuery = _cvDbContext.CVs.Where(filter);
                    cvQuery = isDescending ? cvQuery.OrderByDescending(sort) : cvQuery.OrderBy(sort);
                    cvs = await cvQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (cvs == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    foreach (var cv in cvs)
                    {
                        UserModel user = _userService.GetUser(userId).Result.Item2;
                        cv.User = user;
                    }
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, cvs);
        }

        public async Task<Status> Update(CVModel cvModel)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;

            do
            {
                if (cvModel == null || userId <= 0)
                {
                    result.ErrorCode = ErrorCode.cvNull;
                    break;
                }
              
                if (cvModel.UserId != userId)
                {
                    result.ErrorCode = ErrorCode.NoPrivilege;
                    break;
                }

                try
                {
                    var cv = await _cvDbContext.CVs.Where(x => x.Id == cvModel.Id).FirstOrDefaultAsync();
                    PropertyHelper.InjectNonNull<CVModel>(cv, cvModel);
                    _cvDbContext.Update(cv);
                    await _cvDbContext.SaveChangesAsync();


                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<Status> SetCVAsDefault(int cvId)
        {
            Status result = new Status();
            CVModel cv = null;
            int userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    if (cvId <= 0 || userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    cv = await _cvDbContext.CVs.FirstOrDefaultAsync(x => x.Id == cvId);
                    if (cv == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    if (cv.UserId != userId)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }
                    await _userService.UpdateUserDefaultCV(userId, cvId);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, CVModel)> GetDefaultCV(int userId)
        {
            Status result = new Status();
            CVModel cv = null;
            UserModel user = null;

            do
            {
                try
                {
                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    user = _userService.GetUser(userId).Result.Item2;
                    if (user == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    if (user.DefaultCVId == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }

                    cv = await _cvDbContext.CVs.Where(x => x.Id == user.DefaultCVId).FirstOrDefaultAsync();
                    if (cv == null)
                    {
                        result.ErrorCode = ErrorCode.cvNull;
                        break;
                    }
                    cv.User = user;

                    
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, cv);
        }

        public async Task<Status> DeleteDefaultCV(int userId)
        {
            Status status = new Status();

            do
            {
                try
                {
                    if (userId <= 0)
                    {
                        status.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }

                    await _userService.DeleteUserDefaultCV(userId);
                }
                catch (Exception e)
                {
                    status.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (status);
        }
    }
}
