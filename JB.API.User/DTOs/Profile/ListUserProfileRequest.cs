using JB.User.Models.Profile;
using System;
using System.Linq.Expressions;
using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;

namespace JB.User.DTOs.Profile
{
    public class ListUserProfileRequest : ListVM<UserProfileModel>, ISearchRequest
    {
        public int? OrganizationId { get; set; }
        public int? RoleId { get; set; }
        public string Keyword { get; set; }
        public string[] City { get; set; }
        public string[] Country { get; set; }
        public string[] Gender { get; set; }
        public string[] Skills { get; set; }
        public int? UserId { get; set; }

        public override Expression<Func<UserProfileModel, bool>> GetFilterExpression()
        {
            Expression<Func<UserProfileModel, bool>> filter = ExpressionHelper.True<UserProfileModel>();

            if (OrganizationId > 0)
            {
                filter = filter.And(b => b.OrganizationId == OrganizationId);
            }

            if (RoleId > 0)
            {
                filter = filter.And(b => b.RoleId == RoleId);
            }

            return filter;
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(UserProfileModel.Id), nameof(UserProfileModel.Name) };
        }
    }
}
