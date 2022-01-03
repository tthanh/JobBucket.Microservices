using JB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.Services.Search
{
    public interface ISearchService<T>
    {
        Task<(Status, List<T>)> Search(string keyword, Expression<Func<T, bool>> filter, Expression<Func<T, object>> sort, int size, int offset, bool isDescending = false);
        Task<(Status, List<T>)> Search(T entity, Expression<Func<T, bool>> filter, Expression<Func<T, object>> sort, int size, int offset, bool isDescending = false);
    }
}
