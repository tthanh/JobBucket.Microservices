using AutoMapper;
using JB.gRPC.Profile;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Elasticsearch.User;
using JB.Infrastructure.Helpers;
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
    public class UserProfileDocumentElasticsearchService : IUserProfileDocumentElasticsearchService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileElasticsearchService> _logger;
        private readonly IUserClaimsModel _claims;
        private readonly IJobService _jobService;

        private readonly Nest.IElasticClient _elasticClient;

        public UserProfileDocumentElasticsearchService(
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

        public async Task<UserProfileDocument> AddAsync(UserProfileModel profile)
        {
            UserProfileDocument doc = _mapper.Map<UserProfileDocument>(profile);
            await _elasticClient.IndexAsync(doc, r => r.Index("profile"));

            return doc;
        }

        public async Task<UserProfileDocument> UpdateAsync(UserProfileModel profile)
        {
            UserProfileDocument doc = _mapper.Map<UserProfileDocument>(profile);
            await _elasticClient.UpdateAsync<UserProfileDocument>(profile.Id, u => u.Index("profile").Doc(doc));

            return doc;
        }
        public async Task DeleteAsync(int id) => await _elasticClient.DeleteAsync<UserProfileModel>(id, r => r.Index("profile"));

        public async Task DeleteIndiceAsync() => await _elasticClient.Indices.DeleteAsync("profile");
    }
}
