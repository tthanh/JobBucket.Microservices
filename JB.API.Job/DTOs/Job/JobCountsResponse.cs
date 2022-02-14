using System.Collections.Generic;

namespace JB.Job.DTOs.Job
{
    public class JobCountsResponse
    {
        public long TotalCount { get; set; }
        public ICollection<JobCountsByResponse> ByCategories { get; set; }
    }
}
