using JB.Infrastructure.Elasticsearch.Job.Property;
using JB.Infrastructure.Elasticsearch.Organization;
using JB.Infrastructure.Elasticsearch.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Infrastructure.Elasticsearch.Job
{
    public class JobDocument
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string[] ImageUrls { get; set; }
        public string Description { get; set; }
        public int ActiveStatus { get; set; }
        public int Priority { get; set; }
        public string[] Addresses { get; set; }
        public string[] Cities { get; set; }
        public int MinSalary { get; set; }
        public int MaxSalary { get; set; }
        public string SalaryCurrency { get; set; }
        public string SalaryDuration { get; set; }
        public ICollection<JobSkillDocument> Skills { get; set; }
        public ICollection<JobPositionDocument> Positions { get; set; }
        public ICollection<JobApplicationDocument> Applications { get; set; }
        public int ApplicationCount { get; set; }
        public ICollection<JobInterestDocument> Interests { get; set; }
        public int InterestCount { get; set; }
        public ICollection<JobTypeDocument> Types { get; set; }
        public ICollection<JobCategoryDocument> Categories { get; set; }
        public bool IsVisaSponsorship { get; set; }
        public int EmployerId { get; set; }
        public UserDocument Employer { get; set; }
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
        public OrganizationDocument Organization { get; set; }
    }
}
