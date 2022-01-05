using AutoMapper;
using JB.Lib.Models.User;
using JB.Authentication.DTOs.Authentication;
using JB.Authentication.Models.User;

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
