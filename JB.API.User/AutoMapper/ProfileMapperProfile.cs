using AutoMapper;
using JB.User.AutoMapper.Converters;
using JB.User.Data;
using JB.User.Models.CV;
using JB.User.DTOs.CV;
using JB.User.Models.Profile;
using JB.User.DTOs.Profile;
using JB.User.Models.User;
using JB.Infrastructure.Elasticsearch.User;

namespace JB.User.AutoMapper
{
    public class ProfileMapperProfile : Profile
    {
        public ProfileMapperProfile()
        {
            CreateMap<int?, int>().ConvertUsing((src, dest) => { if (src.HasValue) return src.Value; return dest; });

            CreateMap<UserProfileModel, UserProfileResponse>();

            CreateMap<UpdateUserProfileRequest, UserProfileModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateUserProfileRequest, UserModel>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserProfileModel, UserModel>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserModel, UserProfileModel>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserProfileModel, UserProfileDocument>();

            CreateMap<UserProfileModel, Profile>();
            CreateMap<UserSkillModel, gRPC.Profile.UserExperience>();
            CreateMap<UserEducationModel, gRPC.Profile.UserEducation>();
            CreateMap<UserExperienceModel, gRPC.Profile.UserSkill>();
        }
    }
}
