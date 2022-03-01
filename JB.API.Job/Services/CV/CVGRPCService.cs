using AutoMapper;
using JB.Infrastructure.Models;
using JB.Job.Models.CV;
using JB.Job.Models.Notification;
using JB.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly ILogger<CVGRPCService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;
        private readonly gRPC.CV.CVRPC.CVRPCClient _cvGrpcClient;

        public CVGRPCService(
            ILogger<CVGRPCService> logger,
            IDistributedCache cache,
            IMapper mapper,
            gRPC.CV.CVRPC.CVRPCClient cvGrpcClient
            )
        {
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
            _cvGrpcClient = cvGrpcClient;
        }
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

        public async Task<(Status, CVModel)> GetById(int id)
        {
            Status status = new Status();
            var cv = await _cache.GetAsync<CVModel>($"cv-{id}");

            if (cv == null)
            {
                var req = new gRPC.CV.CVRequest();
                req.Id.Add(id);
                var cvResp = await _cvGrpcClient.GetAsync(req);
                cv = cvResp.Cvs.Count == 1 ? _mapper.Map<CVModel>(cvResp.Cvs[0]) : null;
                if (cv == null)
                {
                    status.ErrorCode = Infrastructure.Constants.ErrorCode.UserNotExist;
                }
            }

            return (status, cv);
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
