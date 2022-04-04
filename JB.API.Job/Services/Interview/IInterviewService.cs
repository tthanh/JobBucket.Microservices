using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using JB.Job.DTOs.Job;
using JB.Job.Models.Interview;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public interface IInterviewService : IServiceBase<InterviewModel>
    {
        Task<(Status, List<(int Status, string StatusName, int Count)>)> GetInterviewCounts(InterviewCountsRequest req);
    }
}
