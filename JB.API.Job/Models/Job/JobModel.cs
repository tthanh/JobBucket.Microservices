using JB.Infrastructure.Models;
using JB.Job.Models.Organization;
using JB.Job.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Job.Models.Job
{
    public class JobModel : IEntityDate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string[] ImageUrls { get; set; }
        public string Description { get; set; }
        public int ActiveStatus { get; set; }
        public int? Priority { get; set; }
        public string[] Addresses { get; set; }
        public string[] Cities { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public string SalaryCurrency { get; set; }
        public string SalaryDuration { get; set; }
        public virtual ICollection<SkillModel> Skills { get; set; }
        public virtual ICollection<PositionModel> Positions { get; set; }
        public virtual ICollection<ApplicationModel> Applications { get; set; }
        [NotMapped]
        public int ApplicationCount { get; set; }
        public virtual ICollection<InterestModel> Interests { get; set; }
        [NotMapped]
        public int InterestCount { get; set; }
        public virtual ICollection<TypeModel> Types { get; set; }
        public virtual ICollection<CategoryModel> Categories { get; set; }
        public bool? IsVisaSponsorship { get; set; }
        public int EmployerId { get; set; }
        [NotMapped]
        public UserModel Employer { get; set; }
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
        public int? NumberEmployeesToApplied { get; set; }
        public string JobForm { get; set; }
        public string Gender { get; set; }
        public int? Views { get; set; }
        [NotMapped]
        public bool IsJobInterested { get; set; }
        [NotMapped]
        public bool IsJobApplied { get; set; }
        public int OrganizationId { get; set; }
        [NotMapped]
        public OrganizationModel Organization { get; set; }
    }
}