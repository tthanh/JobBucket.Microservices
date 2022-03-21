using JB.Blog.Blog.Models;
using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JB.Blog.DTOs.Blog
{
    public class ListBlogRequest : ListVM<BlogModel>, ISearchRequest
    {
        public string[] Tags { get; set; }
        public int? AuthorId { get; set; }
        public DateTime[] CreatedDate { get; set; }
        public string Keyword { get; set; }
        public bool? IsInterested { get; set; }

        public override Expression<Func<BlogModel, bool>> GetFilterExpression()
        {
            Expression<Func<BlogModel, bool>> filter = ExpressionHelper.True<BlogModel>();

            if (Tags?.Length > 0)
            {
                filter = filter.And(b => b.Tags.Any(x => Tags.Contains(x)));
            }

            if (AuthorId > 0)
            {
                filter = filter.And(b => b.AuthorId == AuthorId);
            }

            if (!string.IsNullOrEmpty(Keyword))
            {
                filter = filter.And(b => !string.IsNullOrEmpty(b.Title) && b.Title.ToLower().Contains(Keyword.ToLower()));
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
            return new string[] { nameof(BlogModel.AuthorId), nameof(BlogModel.CreatedDate), nameof(BlogModel.Tags) };
        }
    }
}
