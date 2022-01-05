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
        Task<Status> LockUser(int userId, DateTime lockUntil);
        Task<Status> UnlockUser(int userId);
        public Task<(Status, List<UserModel>)> ListUser(Expression<Func<UserModel, bool>> filters, Expression<Func<UserModel, object>> sorts, int size, int offset);
        Task<(Status, UserModel)> GetUser(int userId);
        Task<(Status, UserModel)> GetUser(string userName, string authSource = null);
        Task<(Status, UserModel)> PromoteTo(int userId, int ogId, int roleId);
        Task<(Status, List<UserModel>)> DemoteToUser(int userId);
        Task<(Status, List<UserModel>)> GetUsers(List<int> userIds);
        Task<(Status, int)> CountUser(Expression<Func<UserModel, bool>> filters);
        Task<(Status, List<int>)> GetUserIds(Expression<Func<UserModel, bool>> filters);
        Task<(Status, UserModel)> UpdateUserDefaultCV(int userId, int cvId);
        Task<(Status, UserModel)> DeleteUserDefaultCV(int userId);
        Task<(Status, UserModel)> CreateUser(UserModel user);
        Task<Status> DeleteUser(UserModel user);
        Task<Status> DeleteUser(int userId);
        Task<(Status, UserModel)> UpdateUser(UserModel user);
    }
}
