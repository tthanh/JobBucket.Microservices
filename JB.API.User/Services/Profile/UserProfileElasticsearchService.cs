using AutoMapper;
using JB.gRPC.Profile;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Services;
using JB.User.DTOs.Profile;
using JB.User.Models.Profile;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Status = JB.Infrastructure.Models.Status;

namespace JB.User.Services
{
    public class UserProfileElasticsearchService : IUserProfileSearchService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileElasticsearchService> _logger;
        private readonly IUserClaimsModel _claims;
        private readonly IJobService _jobService;

        private readonly Nest.IElasticClient _elasticClient;

        public UserProfileElasticsearchService(
            IMapper mapper,
            ILogger<UserProfileElasticsearchService> logger,
            IUserClaimsModel claims,
            IJobService jobService,
            Nest.IElasticClient elasticClient
        )
        {
            _elasticClient = elasticClient;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _jobService = jobService;

        }
        public async Task<(Status, List<UserProfileModel>)> Search(string keyword, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false)
        {
            Status result = new Status();
            var profiles = new List<UserProfileModel>();
            int userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    userId = _claims?.Id ?? userId;

                    var fields = typeof(UserProfileResponse).GetProperties()
                        .Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(ICollection<string>) || p.PropertyType == typeof(string[]))
                        .Select(p => char.ToLowerInvariant(p.Name[0]) + p.Name[1..])
                        .ToArray();

                    var searchResponse = await _elasticClient.SearchAsync<UserProfileModel>(r => r
                        .Index("profile")
                        .From((offset - 1) * size)
                        .Size(size)
                        .Query(q => q.QueryString(qs => qs.Query(keyword))));

                    if (!searchResponse.IsValid)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    profiles = searchResponse.Hits.Select(r => _mapper.Map<UserProfileModel>(r.Source)).ToList();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, profiles);
        }
        public async Task<(Status, List<UserProfileModel>)> Search(int[] entityIds = null, Expression<Func<UserProfileModel, bool>> filter = null, Expression<Func<UserProfileModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false)
        {
            Status result = new Status();
            var profiles = new List<UserProfileModel>();
            int userId = _claims?.Id ?? 0;
            List<string> likeTerms = new List<string>();
            ISearchResponse<UserProfileModel> searchResponse = null;

            do
            {
                try
                {
                    userId = _claims?.Id ?? userId;

                    // Get Job skills, type, category, city, salary range
                    (var getJobtatus, var jobs) = await _jobService.ListByEmployerId(_claims.Id);
                    if (getJobtatus.IsSuccess)
                    {
                        foreach(var job in jobs)
{
                            likeTerms.AddRange(job.Skills?.Select(x => x.Name) ?? Enumerable.Empty<string>());
                            likeTerms.AddRange(job.Types?.Select(x => x.Name) ?? Enumerable.Empty<string>());
                            likeTerms.AddRange(job.Categories?.Select(x => x.Name) ?? Enumerable.Empty<string>());
                            likeTerms.AddRange(job.Cities ?? Enumerable.Empty<string>());
                            likeTerms.AddRange(job.Positions?.Select(x => x.Name) ?? Enumerable.Empty<string>());
}
                    }

                    if (entityIds == null || entityIds.Count() == 0)
                    {
                        searchResponse = await _elasticClient.SearchAsync<UserProfileModel>(r => r
                        .Index("profile")
                        .From(offset * size)
                        .Size(size)
                        .Query(q => q.MultiMatch(mm => mm
                           .Query(string.Join(' ', likeTerms))
                           .Fields(f => f
                               .Fields(
                                   "city",
                                   "country",
                                   "introduction",
                                   "certifications",
                                   "awards",
                                   "skills.skillName",
                                   "educations.major",
                                   "educations.profession",
                                   "experiences.position"
                               )
                           ))
                        ));
                    }
                    else
                    {
                        searchResponse = await _elasticClient.SearchAsync<UserProfileModel>(r => r
                        .Index("job")
                        .From(offset * size)
                        .Size(size)
                        .Query(q => q.MoreLikeThis(mlt => mlt
                            .Like(l => l
                                .Document(ld =>
                                {
                                    ld = ld.Index("job");

                                    foreach (var id in entityIds)
                                    {
                                        ld = ld.Id(id);
                                    }

                                    return ld;
                                })
                                //Like user skill, position, type, category
                                .Text(string.Join(' ', likeTerms))
                            )
                            .Fields(f => f
                                .Fields(
                                    "city",
                                   "country",
                                   "introduction",
                                   "certifications",
                                   "awards",
                                   "skills.skillName",
                                   "educations.major",
                                   "educations.profession",
                                   "experiences.position"
                                   ))
                            .MaxQueryTerms(12)
                            .MinTermFrequency(1)
                        )));
                    }

                    if (!searchResponse.IsValid)
                    {
                        result.ErrorCode = ErrorCode.InvalidData;
                        break;
                    }

                    profiles = searchResponse.Hits.Select(r => _mapper.Map<UserProfileModel>(r.Source)).ToList();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, profiles);
        }

        public Task<(Status, List<UserProfileModel>)> Search(ListUserProfileRequest filter = null)
        {
            throw new NotImplementedException();
        }
    }
}
