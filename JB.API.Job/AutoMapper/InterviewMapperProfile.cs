using AutoMapper;
using JB.Job.Models.Interview;
using JB.Job.Models.Job;
using JB.Job.Models.Organization;
using JB.Job.Models.User;
using JB.Job.DTOs.Interview;
using JB.Job.Models.CV;

namespace JB.Job.AutoMapper
{
    public class InterviewMapperProfile : Profile
    {
        public InterviewMapperProfile()
        {
            CreateMap<int?, int>().ConvertUsing((src, dest) => { if (src.HasValue) return src.Value; return dest; });

            CreateMap<AddInterviewRequest, InterviewModel>();
            CreateMap<UpdateInterviewRequest, InterviewModel>();

            CreateMap<OrganizationModel, InterviewOrganizationResponse>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CVModel, InterviewCVResponse>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UserModel, InterviewUserResponse>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<JobModel, InterviewJobResponse>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<InterviewModel, InterviewResponse>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }


}
