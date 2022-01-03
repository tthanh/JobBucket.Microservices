using JB.Infrastructure.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace JB.Infrastructure.DTOs
{
    public class ListRequest
    {
        [Range(1, int.MaxValue)]
        public int? Page { get; set; }

        [Range(1, 20)]
        public int? Size { get; set; }

        public bool? IsDescending { get; set; }

        public string SortBy { get; set; }

        public virtual Expression<Func<T, object>> GetSortExpression<T>()
        {
            try
            {
                if (string.IsNullOrEmpty(SortBy))
                {
                    return null;
                }

                return ExpressionHelper.GetSortLambda<T>(SortBy);
            }
            catch
            {
                return null;
            }
        }
    }

    public class ListVM<T> : ListRequest where T : class
    {
        public virtual Expression<Func<T, object>> GetSortExpression()
        {
            try
            {
                if (string.IsNullOrEmpty(SortBy) || !(GetAllowedSortFields()?.Any(f => string.Equals(SortBy,f, StringComparison.OrdinalIgnoreCase)) ?? false))
                {
                    return u => u;
                }

                return ExpressionHelper.GetSortLambda<T>(SortBy);
            }
            catch
            {
                return u => u;
            }
        }

        public virtual Expression<Func<T, bool>> GetFilterExpression()
        {
            return ExpressionHelper.True<T>();
        }

        protected virtual string[] GetAllowedSortFields()
        {
            return Array.Empty<string>();
        }
    }

}