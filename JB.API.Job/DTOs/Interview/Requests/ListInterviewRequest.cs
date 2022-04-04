using JB.Job.Models.Interview;
using System;
using System.Linq;
using System.Linq.Expressions;
using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;

namespace JB.Job.DTOs.Job
{
    public class ListInterviewRequest : ListVM<InterviewModel>, ISearchRequest
    {
        public int? Status { get; set; }
        public int? JobId { get; set; }
        public int? InterviewerId { get; set; }
        public int? IntervieweeId { get; set; }
        public DateTime[] InterviewTime { get; set; }
        public DateTime[] CreatedDate { get; set; }
        public string Keyword { get; set; }

        public override Expression<Func<InterviewModel, bool>> GetFilterExpression()
        {
            Expression<Func<InterviewModel, bool>> filter = ExpressionHelper.True<InterviewModel>();

            if (JobId > 0)
            {
                filter = filter.And(b => b.JobId == JobId);
            }

            if (InterviewerId > 0)
            {
                filter = filter.And(b => b.InterviewerId == InterviewerId);
            }

            if (IntervieweeId > 0)
            {
                filter = filter.And(b => b.IntervieweeId == IntervieweeId);
            }

            if (Status > 0)
            {
                filter = filter.And(b => b.Status == Status);
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

            if (InterviewTime != null)
            {
                Array.Sort(InterviewTime);

                DateTime lowValue = InterviewTime.ElementAtOrDefault(0);
                DateTime highValue = InterviewTime.ElementAtOrDefault(1);

                lowValue = lowValue == default ? DateTime.MinValue : lowValue;
                highValue = highValue == default ? DateTime.MaxValue : highValue;

                filter = filter.And(j => j.InterviewTime >= lowValue && j.InterviewTime <= highValue);
            }

            return filter;
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(InterviewModel.InterviewTime), nameof(InterviewModel.CreatedDate) };
        }
    }
}
