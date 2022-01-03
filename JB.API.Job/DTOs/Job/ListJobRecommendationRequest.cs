using JB.Job.Models.Job;
using System;
using JB.Infrastructure.DTOs;

namespace JB.Job.DTOs.Job
{
    public class ListJobRecommendationRequest : ListVM<JobModel>
    {
        public int? JobId { get; set; }
        public int[] Salary { get; set; }
        public int[] Position { get; set; }
        public int[] Skill { get; set; }
        public int[] Category { get; set; }
        public int[] Type { get; set; }
        public string[] Cities { get; set; }
        public DateTime[] ExpireDate { get; set; }
        public DateTime[] CreatedDate { get; set; }
        public int[] OrganizationId { get; set; }
        public bool? IsInterested { get; set; }
    }
}
