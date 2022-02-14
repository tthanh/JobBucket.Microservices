using System;
using System.Collections.Generic;
using JB.Infrastructure.Helpers;
using JB.Job.Constants;
using JB.Job.DTOs.Job.Property;

namespace JB.Job.DTOs.Job
{
    public class JobResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string[] ImageUrls { get; set; }
        public string Description { get; set; }
        public int ActiveStatus { get; set; }
        public string ActiveStatusDisplay { get => EnumHelper.GetDescriptionFromEnumValue((JobActiveStatus)ActiveStatus); }
        public int Priority { get; set; }
        public string[] Addresses { get; set; }
        public string[] Cities { get; set; }
        public int MinSalary { get; set; }
        public int MaxSalary { get; set; }
        public string SalaryCurrency { get; set; }
        public string SalaryDuration { get; set; }
        public ICollection<JobSkillResponse> Skills { get; set; }
        public ICollection<JobPositionResponse> Positions { get; set; }
        public ICollection<ApplicationResponse> Applications { get; set; }
        public int ApplicationCount { get; set; }
        public ICollection<InterestResponse> Interests { get; set; }
        public int InterestCount { get; set; }
        public ICollection<JobTypeResponse> Types { get; set; }
        public ICollection<JobCategoryResponse> Categories { get; set; }
        public bool IsVisaSponsorship { get; set; }
        public int EmployerId { get; set; }
        public JobUserResponse Employer { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Benefits { get; set; }
        public string Experiences { get; set; }
        public string Responsibilities { get; set; }
        public string Requirements { get; set; }
        public string OptionalRequirements { get; set; }
        public string Cultures { get; set; }
        public string WhyJoinUs { get; set; }
        public int NumberEmployeesToApplied { get; set; }
        public string JobForm { get; set; }
        public string Gender { get; set; }
        public int Views { get; set; }
        public bool IsJobInterested { get; set; }
        public bool IsJobApplied { get; set; }
        public int OrganizationId { get; set; }
        public JobOrganizationResponse Organization { get; set; }
    }
}
