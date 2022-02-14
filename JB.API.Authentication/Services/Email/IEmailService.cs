using JB.Infrastructure.Models;
using System.Threading.Tasks;

namespace JB.Authentication.Services
{
    public interface IEmailService
    {
        Task<Status> SendEmailAsync(string email, string subject, string message, bool isBodyHtml = true);
    }
}
