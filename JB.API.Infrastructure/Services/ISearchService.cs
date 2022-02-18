using JB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Infrastructure.Services
{
    public interface ISearchService<T>
    {
        Task<(Status, List<T>)> Search(string keyword, Expression<Func<T, bool>> filter = null, Expression<Func<T, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
        Task<(Status, List<T>)> Search(int[] entityIds = null, Expression<Func<T, bool>> filter = null, Expression<Func<T, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
    }
}
