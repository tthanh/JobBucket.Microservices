using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using JB.User.Models.Profile;
using System.Threading.Tasks;

namespace JB.User.Services
{
    public interface IUserProfileService : IServiceBase<UserProfileModel>
    {
        public Task<(Status, UserProfileModel)> GetOrCreate(int id);
    }
}
