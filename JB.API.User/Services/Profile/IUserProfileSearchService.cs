using AutoMapper;
using JB.gRPC.Profile;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Services;
using JB.User.DTOs.Profile;
using JB.User.Models.Profile;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Status = JB.Infrastructure.Models.Status;

namespace JB.User.Services
{
    public interface IUserProfileSearchService
    {
        Task<(Status, List<UserProfileModel>)> Search(string keyword, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
        Task<(Status, List<UserProfileModel>)> Search(int[] entityIds = null, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
        Task<(Status, List<UserProfileModel>)> Search(ListUserProfileRequest filter = null);
    }
}
