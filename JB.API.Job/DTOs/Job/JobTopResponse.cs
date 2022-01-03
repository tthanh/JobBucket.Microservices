using System.Collections.Generic;

namespace JB.Job.DTOs.Job
{
    public class JobTopResponse
    {
        public ICollection<JobResponse> MostViewed { get; set; }
        public ICollection<JobResponse> MostInterested { get; set; }
        public ICollection<JobResponse> Newest { get; set; }
        public ICollection<JobResponse> MostRelevant { get; set; }
    }
}
