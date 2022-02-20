using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using JB.Job.Models.Organization;
using JB.Job.Models.Profile;
using JB.Job.Models.User;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public interface IUserProfileService
    {
        Task<(Status, UserProfileModel)> GetById(int id);
    }
}
