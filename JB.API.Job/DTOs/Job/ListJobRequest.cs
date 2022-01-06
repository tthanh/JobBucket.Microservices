using JB.Job.Models.Job;
using System;
using System.Linq;
using System.Linq.Expressions;
using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;

namespace JB.Job.DTOs.Job
{
    public class ListJobRequest : ListVM<JobModel>, ISearchRequest
    {
        public int[] NumberEmployeesToApplied { get; set; }
        public int[] Salary { get; set; }
        public int[] Position { get; set; }
        public int[] Skill { get; set; }
        public int[] ActiveStatus { get; set; }
        public int[] Category { get; set; }
        public int[] Type { get; set; }
        public string[] Cities { get; set; }
        public DateTime[] ExpireDate { get; set; }
        public DateTime[] CreatedDate { get; set; }
        public int[] OrganizationId { get; set; }
        public int[] EmployerId { get; set; }
        public string Keyword { get; set; }
        public bool? IsInterested { get; set; }

        public override Expression<Func<JobModel, bool>> GetFilterExpression()
        {
            Expression<Func<JobModel, bool>> filter = ExpressionHelper.True<JobModel>();

            if (NumberEmployeesToApplied?.Length > 0)
            {
                Array.Sort(NumberEmployeesToApplied);

                int lowValue = NumberEmployeesToApplied.ElementAtOrDefault(0);
                int highValue = NumberEmployeesToApplied.ElementAtOrDefault(1);

                lowValue = lowValue == default ? int.MinValue : lowValue;
                highValue = highValue == default ? int.MaxValue : highValue;

                filter = filter.And(j => j.NumberEmployeesToApplied >= lowValue && j.NumberEmployeesToApplied <= highValue);
            }

            if (Salary?.Length > 0)
            {
                Array.Sort(Salary);

                int lowValue = Salary.ElementAtOrDefault(0);
                int highValue = Salary.ElementAtOrDefault(1);

                lowValue = lowValue == default ? int.MinValue : lowValue;
                highValue = highValue == default ? int.MaxValue : highValue;

                filter = filter.And(j => (j.MaxSalary >= lowValue && j.MaxSalary <= highValue) || (j.MinSalary >= lowValue && j.MinSalary <= highValue));
            }

            if (Position?.Length > 0)
            {
                filter = filter.And(j => j.Positions.Any(p => Position.Contains(p.Id)));
            }

            if (Skill?.Length > 0)
            {
                filter = filter.And(j => j.Skills.Any(p => Skill.Contains(p.Id)));
            }

            if (ActiveStatus?.Length > 0)
            {
                filter = filter.And(j => ActiveStatus.Contains(j.ActiveStatus));
            }

            if (OrganizationId?.Length > 0)
            {
                filter = filter.And(j => OrganizationId.Contains(j.OrganizationId));
            }

            if (EmployerId?.Length > 0)
            {
                filter = filter.And(j => EmployerId.Contains(j.EmployerId));
            }

            if (Cities?.Length > 0)
            {
                filter = filter.And(j => j.Cities.Any(c => Cities.Contains(c)));
            }

            if (Category?.Length > 0)
            {
                filter = filter.And(j => j.Categories.Any(c => Category.Contains(c.Id)));
            }

            if (Type?.Length > 0)
            {
                filter = filter.And(j => j.Types.Any(c => Type.Contains(c.Id)));
            }

            if (ExpireDate != null)
            {
                Array.Sort(ExpireDate);

                DateTime lowValue = ExpireDate.ElementAtOrDefault(0);
                DateTime highValue = ExpireDate.ElementAtOrDefault(1);

                lowValue = lowValue == default ? DateTime.MinValue : lowValue;
                highValue = highValue == default ? DateTime.MaxValue : highValue;

                filter = filter.And(j => j.ExpireDate >= lowValue && j.ExpireDate <= highValue);
            }

            if (CreatedDate != null)
            {
                Array.Sort(CreatedDate);

                DateTime lowValue = CreatedDate.ElementAtOrDefault(0);
                DateTime highValue = CreatedDate.ElementAtOrDefault(1);

                lowValue = lowValue == default ? DateTime.MinValue : lowValue;
                highValue = highValue == default ? DateTime.MaxValue : highValue;

                filter = filter.And(j => j.CreatedDate >= lowValue && j.CreatedDate <= highValue);
            }

            return filter;
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(JobModel.CreatedDate), nameof(JobModel.UpdatedDate), nameof(JobModel.MaxSalary), nameof(JobModel.OrganizationId) };
        }
    }
}