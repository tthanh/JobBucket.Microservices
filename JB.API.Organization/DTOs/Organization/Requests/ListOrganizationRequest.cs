using JB.Organization.Models.Job;
using JB.Organization.Models.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;

namespace JB.Organization.DTOs.Organization
{
    public class ListOrganizationRequest : ListVM<OrganizationModel>, ISearchRequest
    {
        public string Country { get; set; }
        public string Keyword { get; set; }
   
        public override Expression<Func<OrganizationModel, bool>> GetFilterExpression()
        {
            Expression<Func<OrganizationModel, bool>> filter = ExpressionHelper.True<OrganizationModel>();

            if (!string.IsNullOrEmpty(Country))
            {
                filter = filter.And(o => o.Country.Contains(Country));
            }

            return filter;
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(OrganizationModel.Name) };
        }
    }
}