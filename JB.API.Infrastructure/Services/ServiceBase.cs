using JB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Infrastructure.Services
{
    public interface IServiceBase<T> where T : class
    {
        Task<(Status, long)> Count(Expression<Func<T, bool>> predicate);
        Task<(Status, List<T>)> List(Expression<Func<T, bool>> filter, Expression<Func<T, object>> sort, int size, int offset, bool isDescending = false);
        Task<(Status, T)> GetById(int id);
        Task<Status> Add(T entity);
        Task<Status> Update(T entity);
        Task<Status> Delete(int id);
    }
}

