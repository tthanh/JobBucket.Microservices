﻿using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;
using JB.Organization.Models.Review;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace JB.Organization.DTOs.Review.Requests
{
    public class ListReviewRequest : ListVM<ReviewModel>, ISearchRequest
    {
        public string Keyword { get; set; }
        public int[] OrganizationId { get; set; }

        public override Expression<Func<ReviewModel, bool>> GetFilterExpression()
        {
            Expression<Func<ReviewModel, bool>> filter = ExpressionHelper.True<ReviewModel>();

            if (OrganizationId?.Length > 0)
            {
                filter = filter.And(x => OrganizationId.Contains(x.OrganizationId));
            }

            return filter;
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(ReviewModel.OrganizationId) };
        }
    }
}