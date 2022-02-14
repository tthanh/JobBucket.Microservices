using JB.User.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using JB.User.DTOs.CV;
using AutoMapper;
using HotChocolate.Resolvers;
using HotChocolate;
using JB.User.Models.CV;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Models;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Constants;

namespace JB.User.GraphQL.CV
{
    public class CVMutation
    {
        private readonly IMapper _mapper;
        private readonly ICVService _cvService;
        private readonly IUserClaimsModel _claims;
        public CVMutation(
            IMapper mapper,
            ICVService cvService,
            IUserClaimsModel claims)
        {
            _mapper = mapper;
            _claims = claims;
            _cvService = cvService;
        }

        public async Task<CVResponse> Add(IResolverContext context, [GraphQLName("cv")] AddCVRequest cvRequest)
        {
            Status status = new();
            CVResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(cvRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                var cv = _mapper.Map<CVModel>(cvRequest);
                if (cv == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _cvService.Add(cv);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<CVResponse>(cv);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<CVResponse> Update(IResolverContext context, [GraphQLName("cv")] UpdateCVRequest cvRequest)
        {
            Status status = new();
            CVModel cv = null;
            CVResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(cvRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, cv) = await _cvService.GetById(cvRequest.Id);
                if (!status.IsSuccess)
                {
                    break;
                }

                cv = _mapper.Map(cvRequest, cv);
                if (cv == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _cvService.Update(cv);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<CVResponse>(cv);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<CVResponse> Delete(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _cvService.Delete(id);
                if (!status.IsSuccess)
                {
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
    }
}
