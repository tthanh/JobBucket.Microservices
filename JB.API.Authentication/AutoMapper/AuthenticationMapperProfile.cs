using AutoMapper;
using JB.Authentication.DTOs.Authentication;
using JB.Authentication.Models.User;
using JB.Infrastructure.Models.Authentication;

namespace JB.Authentication.AutoMapper
{
    public class AuthenticationMapperProfile : Profile
    {
        public AuthenticationMapperProfile()
        {
            CreateMap<RegisterRequest, UserModel>()
                .ForMember(m => m.UserName, o => o.MapFrom(vm => vm.Email))
                .ForMember(m => m.PasswordPlain, o => o.MapFrom(vm => vm.Password));
            CreateMap<UserModel, LoginUserResponse>();
            CreateMap<UserModel, UserClaimsModel>();
        }
    }
}
