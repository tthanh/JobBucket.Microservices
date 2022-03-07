using AutoMapper;
using Elasticsearch.Net;
using JB.gRPC.CV;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Services;
using JB.Job.Data;
using JB.Job.DTOs.Job;
using JB.Job.Models.Job;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Status = JB.Infrastructure.Models.Status;
using JB.Infrastructure.Helpers;

namespace JB.Job.Services
{
    public class JobElasticsearchService : IJobSearchService
    {
        private readonly JobDbContext _jobDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<JobSearchService> _logger;
        private readonly IUserClaimsModel _claims;
        private readonly IUserProfileService _profileService;

        private readonly Nest.IElasticClient _elasticClient;

        public JobElasticsearchService(
            JobDbContext jobDbContext,
            IMapper mapper,
            ILogger<JobSearchService> logger,
            IUserClaimsModel claims,
            IUserProfileService profileService,
            Nest.IElasticClient elasticClient
        )
        {
            _elasticClient = elasticClient;
            _jobDbContext = jobDbContext;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _profileService = profileService;
        }
        public async Task<(Status, List<JobModel>)> Search(string keyword = null, Expression<Func<JobModel, bool>> filter = null, Expression<Func<JobModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false)
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
                        .Sort(ss => ss
                            .Ascending(sort)
                            )
                        .From((offset - 1) * size)
                        .Size(size)
                        .Query(q => q
                            .QueryString(qs => qs
                                .Query(keyword)
                                )
                            )
                        );

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

        public async Task<(Status, List<JobModel>)> Search(int[] entityIds = null, Expression<Func<JobModel, bool>> filter = null, Expression<Func<JobModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false)
        {
            Status result = new Status();
            var jobs = new List<JobModel>();
            int userId = _claims?.Id ?? 0;
            List<string> likeTerms = new List<string>();
            ISearchResponse<JobModel> searchResponse = null;

            do
            {
                try
                {
                    userId = _claims?.Id ?? userId;

                    // Get user's like & apply
                    var appliedJobs = (await _jobDbContext.Application.Where(x => x.UserId == _claims.Id).ToListAsync()).Select(x => x.JobId);
                    var likedJobs = (await _jobDbContext.Interests.Where(x => x.UserId == _claims.Id).ToListAsync()).Select(x => x.JobId);
                    // Get Job skills, type, category, city, salary range
                    var likedAndAppliedJobIds = appliedJobs.Concat(likedJobs).ToList();
                    if (entityIds != null)
                    {
                        likedAndAppliedJobIds.AddRange(entityIds);
                    }

                    likedAndAppliedJobIds = likedAndAppliedJobIds.Distinct().ToList();

                    // Get user's skills
                    (var getProfileStatus, var profile) = await _profileService.GetById(_claims.Id);
                    if (getProfileStatus.IsSuccess)
                    {
                        likeTerms.Add(profile.City);
                        likeTerms.Add(profile.Country);
                        likeTerms.AddRange(profile?.Skills.Select(x => x.SkillName) ?? Enumerable.Empty<string>());
                        likeTerms.AddRange(profile?.Educations.Select(x => x.Major) ?? Enumerable.Empty<string>());
                        likeTerms.AddRange(profile?.Educations.Select(x => x.Profession) ?? Enumerable.Empty<string>());
                        likeTerms.AddRange(profile?.Experiences.Select(x => x.Position) ?? Enumerable.Empty<string>());

                        likeTerms = likeTerms.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                    }

                    if (likedAndAppliedJobIds.Count == 0)
                    {
                        searchResponse = await _elasticClient.SearchAsync<JobModel>(r => r
                        .Index("job")
                        .From(offset * size)
                        .Size(size)
                        .Sort(ss => ss
                            .Ascending(sort)
                            )
                        .Query(q => q.MultiMatch(mm => mm
                           .Query(string.Join(' ', likeTerms))
                           .Fields(f => f
                               .Fields(
                                   "skills.name",
                                   "organization.name",
                                   "positions.name",
                                   "categories.name",
                                   "title",
                                   "description",
                                   "types",
                                   "cities",
                                   "benefits",
                                   "experiences",
                                   "responsibilities",
                                   "requirements"
                               )
                           ))
                        ));
                    }
                    else
                    {
                        searchResponse = await _elasticClient.SearchAsync<JobModel>(r => r
                        .Index("job")
                        .From(offset * size)
                        .Size(size)
                        .Sort(ss => ss
                            .Ascending(sort)
                            )
                        .Query(q => q.MoreLikeThis(mlt => mlt
                            .Like(l => l
                                .Document(ld =>
                                {
                                    ld = ld.Index("job");

                                    foreach (var id in likedAndAppliedJobIds)
                                    {
                                        ld = ld.Id(id);
                                    }

                                    return ld;
                                })
                                //Like user skill, position, type, category
                                .Text(string.Join(' ', likeTerms))
                            )
                            .Fields(f => f
                                .Fields("skills.name",
                                    "organization.name",
                                    "positions.name",
                                    "categories.name",
                                    "title",
                                    "description",
                                    "types",
                                    "cities",
                                    "benefits",
                                    "experiences",
                                    "responsibilities",
                                    "requirements"))
                            .MaxQueryTerms(12)
                            .MinTermFrequency(1)
                        )));
                    }

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

        public async Task<(Status, List<JobModel>)> Search(ListJobRequest filter = null)
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

                    var boolQuery = new BoolQuery
                    {
                        Must = Array.Empty<QueryContainer>()
                    };

                    boolQuery = boolQuery
                        .AppendToMustQuery(new QueryStringQuery
                        {
                            Query = filter.Keyword,
                        })
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("skills.id", filter.Skill))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("types.id", filter.Type))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("positions.id", filter.Position))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("categories.id", filter.Category))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("activeStatus", filter.ActiveStatus))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("cities", filter.Cities));
         
                    var searchRequest = new SearchRequest<JobModel>
                    {
                        From = (filter.Page - 1) * filter.Size,
                        Size = filter.Size,
                        Query = boolQuery,
                    };

                    if (!string.IsNullOrEmpty(filter.SortBy))
                    {
                        searchRequest.Sort = new ISort[] {
                            new FieldSort()
                            {
                                Field = PropertyHelper.ToCamelCase(filter.SortBy),
                                Order = filter.IsDescending == true ? SortOrder.Descending : SortOrder.Ascending,
                            },
                        };
                    }

                    var json = _elasticClient.RequestResponseSerializer.SerializeToString(searchRequest);

                    var searchResponse = await _elasticClient.SearchAsync<JobModel>(searchRequest);

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

        public async Task<(Status, List<JobModel>)> Search(ListJobRecommendationRequest filter = null)
        {
            Status result = new Status();
            var jobs = new List<JobModel>();
            int userId = _claims?.Id ?? 0;
            List<string> likeTerms = new List<string>();
            ISearchResponse<JobModel> searchResponse = null;

            do
            {
                try
                {
                    userId = _claims?.Id ?? userId;

                    // Get user's like & apply
                    var appliedJobs = (await _jobDbContext.Application.Where(x => x.UserId == _claims.Id).ToListAsync()).Select(x => x.JobId);
                    var likedJobs = (await _jobDbContext.Interests.Where(x => x.UserId == _claims.Id).ToListAsync()).Select(x => x.JobId);
                    // Get Job skills, type, category, city, salary range
                    var likedAndAppliedJobIds = appliedJobs.Concat(likedJobs).ToList();
                    if (filter.JobId != null)
                    {
                        likedAndAppliedJobIds.Add(filter.JobId.Value);
                    }

                    likedAndAppliedJobIds = likedAndAppliedJobIds.Distinct().ToList();

                    // Get user's skills
                    (var getProfileStatus, var profile) = await _profileService.GetById(_claims.Id);
                    if (getProfileStatus.IsSuccess)
                    {
                        likeTerms.Add(profile.City);
                        likeTerms.Add(profile.Country);
                        likeTerms.AddRange(profile?.Skills.Select(x => x.SkillName) ?? Enumerable.Empty<string>());
                        likeTerms.AddRange(profile?.Educations.Select(x => x.Major) ?? Enumerable.Empty<string>());
                        likeTerms.AddRange(profile?.Educations.Select(x => x.Profession) ?? Enumerable.Empty<string>());
                        likeTerms.AddRange(profile?.Experiences.Select(x => x.Position) ?? Enumerable.Empty<string>());

                        likeTerms = likeTerms.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
                    }

                    var boolQuery = new BoolQuery
                    {
                        Must = Array.Empty<QueryContainer>()
                    };

                    boolQuery = boolQuery
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("skills.id", filter.Skill))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("types.id", filter.Type))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("positions.id", filter.Position))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("categories.id", filter.Category))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("activeStatus", filter.ActiveStatus))
                        .AppendToMustQuery(ElasticsearchHelper.GetContainQuery("cities", filter.Cities));

                    if (likedAndAppliedJobIds.Count == 0)
                    {
                        boolQuery = boolQuery.AppendToMustQuery(new MultiMatchQuery
                        {
                            Query = string.Join(' ', likeTerms),
                            Fields = new Field[]
                            {
                                "skills.name",
                                "organization.name",
                                "positions.name",
                                "categories.name",
                                "title",
                                "description",
                                "types",
                                "cities",
                                "benefits",
                                "experiences",
                                "responsibilities",
                                "requirements"
                            }
                        });
                    }
                    else
                    {
                        var mlt = new MoreLikeThisQuery
                        {
                            Like = new Like[]
                            {
                                new Like(string.Join(' ', likeTerms)),
                            },
                            MinTermFrequency = 1,
                            MaxQueryTerms = 12,
                            Fields = new Field[]
                            {
                                "skills.name",
                            "organization.name",
                            "positions.name",
                            "categories.name",
                            "title",
                            "description",
                            "types",
                            "cities",
                            "benefits",
                            "experiences",
                            "responsibilities",
                            "requirements"
                            }
                        };
                        mlt.Like = mlt.Like.Concat(likedAndAppliedJobIds.Select(x => new Like(new LikeDocument<JobModel>(x))));

                        boolQuery = boolQuery.AppendToMustQuery(mlt);
                    }

                    var searchRequest = new SearchRequest<JobModel>
                    {
                        From = (filter.Page - 1) * filter.Size,
                        Size = filter.Size,
                        Query = boolQuery,
                    };

                    if (!string.IsNullOrEmpty(filter.SortBy))
                    {
                        searchRequest.Sort = new ISort[] {
                            new FieldSort()
                            {
                                Field = PropertyHelper.ToCamelCase(filter.SortBy),
                                Order = filter.IsDescending == true ? SortOrder.Descending : SortOrder.Ascending,
                            },
                        };
                    }

                    var json = _elasticClient.RequestResponseSerializer.SerializeToString(searchRequest);

                    searchResponse = await _elasticClient.SearchAsync<JobModel>(searchRequest);

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
