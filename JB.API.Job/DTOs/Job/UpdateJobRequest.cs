using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JB.Job.DTOs.Job
{
    public class UpdateJobRequest
    {
        [Required]
        public int Id { get; set; }
        public string Title { get; set; }
        public string[] ImageUrls { get; set; }
        public string Description { get; set; }
        public int? Priority { get; set; }
        public string[] Addresses { get; set; }
        public string[] Cities { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public string SalaryCurrency { get; set; }
        public string SalaryDuration { get; set; }
        public ICollection<int> SkillIds { get; set; }
        public ICollection<int> PositionIds { get; set; }
        public ICollection<int> TypeIds { get; set; }
        public ICollection<int> CategoryIds { get; set; }
        public bool? IsVisaSponsorship { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Benefits { get; set; }
        public string Experiences { get; set; }
        public string Responsibilities { get; set; }
        public string Requirements { get; set; }
        public string OptionalRequirements { get; set; }
        public string Cultures { get; set; }
        public string WhyJoinUs { get; set; }
        public int? NumberEmployeesToApplied { get; set; }
        public string JobForm { get; set; }
        public string Gender { get; set; }
    }
}
