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

            CreateMap<UserModel, ListUsersResponse>()
               .ForMember(vm => vm.LockoutEnd, o => o.MapFrom(m => DateTimeOffsetToDateTime(m.LockoutEnd)));
             CreateMap<UserModel, GetUserResponse>()
               .ForMember(vm => vm.LockoutEnd, o => o.MapFrom(m => DateTimeOffsetToDateTime(m.LockoutEnd)));

            CreateMap<UserModel, UserDocument>();
            CreateMap<UserModel, JB.gRPC.User.User>();
        }

        private static DateTime? DateTimeOffsetToDateTime(DateTimeOffset? dts)
        {
            return dts.HasValue ? dts.Value.UtcDateTime : null;
        }
    }
}
