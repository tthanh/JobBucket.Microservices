using AutoMapper;
using JB.User.AutoMapper.Converters;
using JB.User.Data;
using JB.User.Models.CV;
using JB.User.DTOs.CV;

namespace JB.User.AutoMapper
{
    public class CVMapperProfile : Profile
    {
        public CVMapperProfile()
        {
            CreateMap<int?, int>().ConvertUsing((src, dest) => { if (src.HasValue) return src.Value; return dest; });

            CreateMap<CVModel, CVResponse>();

            CreateMap<UpdateCVRequest, CVModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AddCVRequest, CVModel>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CVModel, gRPC.CV.CV>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserSkillModel, gRPC.CV.CVSkill>();
            CreateMap<UserEducationModel, gRPC.CV.CVEducation>();
            CreateMap<UserExperienceModel, gRPC.CV.CVExperience>();
        }
    }
}
