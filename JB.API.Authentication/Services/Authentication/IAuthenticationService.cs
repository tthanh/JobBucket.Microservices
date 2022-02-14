using JB.Authentication.Models.User;
using JB.Infrastructure.Models;
using System.Threading.Tasks;

namespace JB.Authentication.Services
{
    public interface IAuthenticationService
    {
        Task<(Status, UserModel)> Authenticate(string username, string password);
        Task<Status> Register(UserModel user);
        Task<Status> ConfirmEmail(string email, string code);
        Task<Status> ResendConfirmationEmail(string username);
        Task<Status> ResetPassword(string email);
        Task<Status> ConfirmResetPassword(string email, string resetToken, string newPassword);
    }
}
