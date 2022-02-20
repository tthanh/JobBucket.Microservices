using JB.API.User.Models.Job;
using JB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.User.Services
{
    public interface IJobService
    {
        Task<(Status, List<JobModel>)> ListByEmployerId(int employerId);
        Task<(Status, List<JobModel>)> ListByIds(int[] ids);
    }
}
