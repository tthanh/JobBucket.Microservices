using AutoMapper;
using JB.gRPC.Profile;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Elasticsearch.User;
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
    public interface IUserProfileDocumentElasticsearchService
    {
        Task<UserProfileDocument> AddAsync(UserProfileModel profile);
        Task<UserProfileDocument> UpdateAsync(UserProfileModel profile);
        Task DeleteAsync(int id);
        Task DeleteIndiceAsync();
    }
}
