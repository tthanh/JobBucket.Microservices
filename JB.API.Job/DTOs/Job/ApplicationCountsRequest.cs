using System.Collections.Generic;

namespace JB.Job.DTOs.Job
{
    public class ApplicationCountsRequest
    {
        public int? Status { get; set; }
        public int? JobId { get; set; }
        public int? EmployerId { get; set; }
        public int? OrganizationId { get; set; }
    }
}
