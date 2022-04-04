using System.Collections.Generic;

namespace JB.Job.DTOs.Job
{
    public class InterviewCountsRequest
    {
        public int? Status { get; set; }
        public int? JobId { get; set; }
        public int? InterviewerId { get; set; }
        public int? OrganizationId { get; set; }
    }
}
