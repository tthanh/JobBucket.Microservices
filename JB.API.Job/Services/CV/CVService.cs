using AutoMapper;
using JB.Infrastructure.Models;
using JB.Job.Models.CV;
using JB.Job.Models.Notification;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public class CVGRPCService : ICVService
    {
        public Task<Status> Add(CVModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, long)> Count(Expression<Func<CVModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, CVModel)> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<(Status, List<CVModel>)> List(Expression<Func<CVModel, bool>> filter, Expression<Func<CVModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            throw new NotImplementedException();
        }

        public Task<Status> Update(CVModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
