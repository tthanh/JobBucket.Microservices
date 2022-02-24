using AutoMapper;
using JB.User.AutoMapper.Converters;
using JB.User.Data;
using JB.User.Models.CV;
using JB.User.DTOs.CV;
using JB.User.Models.Profile;
using JB.User.DTOs.Profile;
using JB.User.Models.User;
using JB.Infrastructure.Elasticsearch.User;
using Google.Protobuf.WellKnownTypes;
using System;
using JB.User.Models.Organization;

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

            CreateMap<DateTime, Timestamp>().ConvertUsing(x => Timestamp.FromDateTime(x.ToUniversalTime()));
            CreateMap<Timestamp, DateTime>().ConvertUsing(x => x.ToDateTime());

            CreateMap<UserProfileModel, gRPC.Profile.Profile>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
                //.ForMember(x => x.Name, x=> x.Condition(x => x != null))
                //.ForMember(x => x.AvatarUrl, x=> x.Condition(x => x != null))
                //.ForMember(x => x.PhoneNumber, x=> x.Condition(x => x != null))
                //.ForMember(x => x.Email, x=> x.Condition(x => x != null))
                //.ForMember(x => x.UserName, x=> x.Condition(x => x != null))
                //.ForMember(x => x.Address, x=> x.Condition(x => x != null))
                //.ForMember(x => x.City, x=> x.Condition(x => x != null))
                //.ForMember(x => x.Country, x=> x.Condition(x => x != null))
                //.ForMember(x => x.Introduction, x=> x.Condition(x => x != null))
                //.ForMember(x => x.Website, x=> x.Condition(x => x != null))
                //.ForMember(x => x.Gender, x=> x.Condition(x => x != null))
                //.ForMember(x => x.Phone, x=> x.Condition(x => x != null))
                //.ForMember(x => x.Reference, x=> x.Condition(x => x != null));

            CreateMap<UserSkillModel, gRPC.Profile.UserSkill>();
            CreateMap<UserEducationModel, gRPC.Profile.UserEducation>();
            CreateMap<UserExperienceModel, gRPC.Profile.UserExperience>();

            CreateMap<gRPC.User.User, UserModel>();
            CreateMap<gRPC.Organization.Organization, OrganizationModel>();
        }
    }
}
