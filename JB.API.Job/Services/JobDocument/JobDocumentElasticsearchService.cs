using AutoMapper;
using JB.Job.Models.Job;
using System.Threading.Tasks;
using JB.Infrastructure.Elasticsearch.Job;

namespace JB.Job.Services
{
    public class JobDocumentElasticsearchService : IJobDocumentElasticsearchService
    {
        private readonly IMapper _mapper;
        private readonly Nest.IElasticClient _elasticClient;

        public JobDocumentElasticsearchService(
            IMapper mapper,
            Nest.IElasticClient elasticClient
        )
        {
            _elasticClient = elasticClient;
            _mapper = mapper;
        }

        public async Task<JobDocument> AddAsync(JobModel job)
        {
            JobDocument doc = _mapper.Map<JobDocument>(job);
            await _elasticClient.IndexAsync(doc, r => r.Index("job"));

            return doc;
        }

        public async Task<JobDocument> UpdateAsync(JobModel job)
        {
            JobDocument doc = _mapper.Map<JobDocument>(job);
            await _elasticClient.UpdateAsync<JobDocument>(job.Id, u => u.Index("job").Doc(doc));

            return doc;
        }

        public async Task DeleteAsync(int id) => await _elasticClient.DeleteAsync<JobModel>(id, r => r.Index("job"));
        public async Task DeleteIndiceAsync() => await _elasticClient.Indices.DeleteAsync("job");
    }
}
