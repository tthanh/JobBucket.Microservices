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
        public string Keyword { get; set; }

        public override Expression<Func<ApplicationModel, bool>> GetFilterExpression()
        {
            Expression<Func<ApplicationModel, bool>> filter = ExpressionHelper.True<ApplicationModel>();

            if (Status > -1)
            {
                filter = filter.And(a => a.Status == Status);
            }

            return filter;
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(ApplicationModel.CreatedDate), nameof(ApplicationModel.Status)};
        }
    }
}