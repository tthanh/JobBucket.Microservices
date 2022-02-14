using AutoMapper;
using JB.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JB.Job.AutoMapper.Converters
{
    public class PrimaryKeyConverter<T, Y> : ITypeConverter<int, T> where T : class, IEntityPrimaryKey where Y : DbContext
    {
        private readonly Y _dbContext;

        public PrimaryKeyConverter(Y dbContext)
        {
            _dbContext = dbContext;
        }

        public T Convert(int source, T destination, ResolutionContext context)
        {
            var entity = _dbContext.Set<T>().FirstOrDefault(e => e.Id == source);
            _dbContext.Set<T>().Attach(entity);

            return entity;
        }
    }
}
