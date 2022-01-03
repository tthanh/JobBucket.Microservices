using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;
using JB.User.Models.CV;
using System;
using System.Linq.Expressions;

namespace JB.User.DTOs.CV
{
    public class ListCVRequest : ListVM<CVModel>, ISearchRequest
    {
        public string Keyword { get; set; }

        public override Expression<Func<CVModel, bool>> GetFilterExpression()
        {
            return ExpressionHelper.True<CVModel>();
        }

        protected override string[] GetAllowedSortFields()
        {
            return new string[] { nameof(CVModel.CreatedDate), nameof(CVModel.UpdatedDate), nameof(CVModel.Name)};
        }
    }
}
