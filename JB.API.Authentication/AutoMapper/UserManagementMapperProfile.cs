using AutoMapper;
using System;
using JB.Authentication.DTOs.UserManagement;
using JB.Authentication.Models.User;
using JB.Infrastructure.Elasticsearch.User;
using Google.Protobuf.WellKnownTypes;

namespace JB.Authentication.AutoMapper
{
    public class UserManagementMapperProfile : Profile
    {
        public UserManagementMapperProfile()
        {
            CreateMap<DateTime, Timestamp>().ConvertUsing(x => Timestamp.FromDateTime(x.ToUniversalTime()));
            CreateMap<UserModel, GetUserResponse>();

            CreateMap<UserModel, UserDocument>();
            CreateMap<UserModel, JB.gRPC.User.User>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }

        private static DateTime? DateTimeOffsetToDateTime(DateTimeOffset? dts)
        {
            return dts.HasValue ? dts.Value.UtcDateTime : null;
        }
    }
}
