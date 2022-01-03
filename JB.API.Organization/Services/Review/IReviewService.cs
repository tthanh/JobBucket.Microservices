using JB.Organization.Models.Review;
using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using System.Threading.Tasks;

namespace JB.Organization.Services
{
    public interface IReviewService : IServiceBase<ReviewModel>
    {
        public Task<(Status, ReviewModel)> Interest(int reviewId);
    }
}
