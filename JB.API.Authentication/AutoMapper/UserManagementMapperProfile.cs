using AutoMapper;
using System;
using JB.Authentication.DTOs.UserManagement;
using JB.Authentication.Models.User;
using JB.Infrastructure.Elasticsearch.User;

namespace JB.Authentication.AutoMapper
{
    public class UserManagementMapperProfile : Profile
    {
        public UserManagementMapperProfile()
        {
            CreateMap<UserModel, ListUsersResponse>()
               .ForMember(vm => vm.LockoutEnd, o => o.MapFrom(m => DateTimeOffsetToDateTime(m.LockoutEnd)));
             CreateMap<UserModel, GetUserResponse>()
               .ForMember(vm => vm.LockoutEnd, o => o.MapFrom(m => DateTimeOffsetToDateTime(m.LockoutEnd)));

            CreateMap<UserModel, UserDocument>();
        }

        private static DateTime? DateTimeOffsetToDateTime(DateTimeOffset? dts)
        {
            return dts.HasValue ? dts.Value.UtcDateTime : null;
        }
    }
}
