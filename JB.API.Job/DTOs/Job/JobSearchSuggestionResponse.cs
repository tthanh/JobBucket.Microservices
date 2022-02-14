using System.Collections.Generic;

namespace JB.Job.DTOs.Job
{
    public class JobSearchSuggestionResponse
    {
        public string[] Keywords { get; set; }
        public ICollection<JobResponse> Jobs { get; set; }
    }
}
