using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using JB.Job.DTOs.Job;
using JB.Job.Models.Interview;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public interface IInterviewService : IServiceBase<InterviewModel>
    {
        Task<(Status, List<(int Status, string StatusName, int Count)>)> GetInterviewCounts(InterviewCountsRequest req);
        public Task<Status> AcceptInterview(int id);
        public Task<Status> DenyInterview(int id);
        public Task<(Status, InterviewModel)> RescheduleInterview(int id, DateTime newDate);
        public Task<(Status, InterviewModel)> PassInterview(int id); // pass pv luôn ko cần pv tiếp
        public Task<(Status, InterviewModel)> FailInterview(int id); // fail pv luôn ko cần pv tiếp
        public Task<(Status, InterviewModel)> NextInterview(int id, DateTime newDate); // đậu vòng hiện tại, xếp lịch pv vòng tiếp theo
    }
}
