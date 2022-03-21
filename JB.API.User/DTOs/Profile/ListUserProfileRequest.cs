using JB.User.Models.Profile;
using System;
using System.Linq.Expressions;
using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;

namespace JB.User.DTOs.Profile
{
    public class ListUserProfileRequest : ListVM<UserProfileModel>, ISearchRequest
    {
        public int? RoleId { get; set; }
        public string Keyword { get; set; }
        public string[] City { get; set; }
        public string[] Skills { get; set; }
        public int? UserId { get; set; }
        public int? JobId { get; set; }

        public override Expression<Func<UserProfileModel, bool>> GetFilterExpression()
        {
            return ExpressionHelper.True<UserProfileModel>();
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(UserProfileModel.Id), nameof(UserProfileModel.Name) };
        }
    }
}
