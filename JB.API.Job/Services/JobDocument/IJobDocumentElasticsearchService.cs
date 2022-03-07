using JB.Infrastructure.Elasticsearch.Job;
using JB.Job.Models.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public interface IJobDocumentElasticsearchService
    {
        Task<JobDocument> AddAsync(JobModel job);
        Task<JobDocument> UpdateAsync(JobModel job);
        Task DeleteAsync(int id);
        Task DeleteIndiceAsync();
    }
}
