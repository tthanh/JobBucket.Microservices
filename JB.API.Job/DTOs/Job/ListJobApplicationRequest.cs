using JB.Job.Models.Job;
using System;
using System.Linq.Expressions;
using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;

namespace JB.Job.DTOs.Job
{
    public class ListJobApplicationRequest : ListVM<ApplicationModel>, ISearchRequest
    {
        public int? Status { get; set; }
        public int? JobId { get; set; }
        public int? EmployerId { get; set; }
        public int? OrganizationId { get; set; }
        public int? UserId { get; set; }
        public string Keyword { get; set; }

        public override Expression<Func<ApplicationModel, bool>> GetFilterExpression()
        {
            Expression<Func<ApplicationModel, bool>> filter = ExpressionHelper.True<ApplicationModel>();

            if (Status > -1)
            {
                filter = filter.And(a => a.Status == Status);
            }

            if (JobId > -1)
            {
                filter = filter.And(a => a.JobId == JobId);
            }

            if (EmployerId > -1)
            {
                filter = filter.And(a => a.Job != null && a.Job.EmployerId == EmployerId);
            }

            if (OrganizationId > -1)
            {
                filter = filter.And(a => a.Job != null && a.Job.OrganizationId == OrganizationId);
            }

            if (UserId > -1)
            {
                filter = filter.And(a => a.UserId == UserId);
            }

            return filter;
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(ApplicationModel.CreatedDate), nameof(ApplicationModel.Status) };
        }
    }
}