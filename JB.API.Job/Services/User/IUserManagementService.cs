using JB.Infrastructure.Models;
using JB.Job.Models.User;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public interface IUserManagementService
    {
        Task<(Status, UserModel)> GetUser(int userId);
    }
}
