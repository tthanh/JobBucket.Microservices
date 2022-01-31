using JB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.Blog.Models.User;

namespace JB.Blog.Services
{
    public interface IUserManagementService
    {
        Task<(Status, UserModel)> GetUser(int userId);
    }
}
