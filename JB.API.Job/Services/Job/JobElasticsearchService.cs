using AutoMapper;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Services;
using JB.Job.Data;
using JB.Job.DTOs.Job;
using JB.Job.Models.Job;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.Services.Job
{
    public class JobElasticsearchService : ISearchService<JobModel>
    {
        private readonly JobDbContext _jobDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<JobSearchService> _logger;
        private readonly IUserClaimsModel _claims;

        private readonly Nest.IElasticClient _elasticClient;

        public JobElasticsearchService(
            JobDbContext jobDbContext,
            IMapper mapper,
            ILogger<JobSearchService> logger,
            IUserClaimsModel claims,
            Nest.IElasticClient elasticClient
        )
        {
            _elasticClient = elasticClient;
            _jobDbContext = jobDbContext;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
        }
        public async Task<(Status, List<JobModel>)> Search(string keyword, Expression<Func<JobModel, bool>> filter, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var jobs = new List<JobModel>();
            int userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    userId = _claims?.Id ?? userId;

                    var fields = typeof(JobResponse).GetProperties()
                        .Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(ICollection<string>) || p.PropertyType == typeof(string[]))
                        .Select(p => char.ToLowerInvariant(p.Name[0]) + p.Name[1..])
                        .ToArray();

                    var searchResponse = await _elasticClient.SearchAsync<JobModel>(r => r
                        .Index("job")
                        .From((offset - 1) * size)
                        .Size(size)
                        .Query(q => q.QueryString(qs => qs.Query(keyword))));

                    if (!searchResponse.IsValid)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    jobs = searchResponse.Hits.Select(r => _mapper.Map<JobModel>(r.Source)).ToList();

                    jobs.ForEach(x =>
                    {
                        x.IsJobApplied = _jobDbContext.Application.Any(i => i.JobId == x.Id && i.UserId == userId);
                        x.IsJobInterested = _jobDbContext.Interests.Any(i => i.JobId == x.Id && i.UserId == userId);
                    });
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, jobs);
        }

        public async Task<(Status, List<JobModel>)> Search(int[] entityIds, Expression<Func<JobModel, bool>> filter, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var jobs = new List<JobModel>();
            int userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    userId = _claims?.Id ?? userId;

                    // Get user's like & apply

                    // Get user's skills

                    // Query
                    var searchResponse = await _elasticClient.SearchAsync<JobModel>(r => r
                        .Index("job")
                        //.From((offset - 1) * size)
                        .From((offset) * size)
                        .Size(size)
                        .Query(q => q.MoreLikeThis(mlt => mlt
                            .Like(l => l.Document(ld => ld.Index("job").Id(entityIds.First())))
                            .Fields(f => f.Fields("skills.name", "organization.name", "positions.name", "categories.name", "title", "description", "types", "cities", "benefits", "experiences", "responsibilities", "requirements"))
                            .MaxQueryTerms(12)
                            .MinTermFrequency(1)
                        )));

                    if (!searchResponse.IsValid)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    jobs = searchResponse.Hits.Select(r => _mapper.Map<JobModel>(r.Source)).ToList();

                    jobs.ForEach(x =>
                    {
                        x.IsJobApplied = _jobDbContext.Application.Any(i => i.JobId == x.Id && i.UserId == userId);
                        x.IsJobInterested = _jobDbContext.Interests.Any(i => i.JobId == x.Id && i.UserId == userId);
                    });
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, jobs);
        }
    }
}
