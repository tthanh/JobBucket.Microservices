using AutoMapper;
using JB.Job.AutoMapper.Converters;
using JB.Job.Data;
using JB.Job.Models.Job;
using JB.Job.Models.Organization;
using JB.Job.Models.User;
using JB.Job.DTOs.Job;
using JB.Job.DTOs.Job.Property;
using JB.Infrastructure.Elasticsearch.Job;
using JB.Infrastructure.Elasticsearch.Job.Property;
using JB.Infrastructure.Elasticsearch.User;
using System;
using Google.Protobuf.WellKnownTypes;

namespace JB.Job.AutoMapper
{
    public class JobMapperProfile : Profile
    {
        public JobMapperProfile()
        {
            CreateMap<int?, int>().ConvertUsing((src, dest) => { if (src.HasValue) return src.Value; return dest; });

            CreateMap<int, PositionModel>()
                .ConvertUsing<PrimaryKeyConverter<PositionModel, JobDbContext>>();

            CreateMap<int, SkillModel>()
                .ConvertUsing<PrimaryKeyConverter<SkillModel, JobDbContext>>();

            CreateMap<int, CategoryModel>()
                .ConvertUsing<PrimaryKeyConverter<CategoryModel, JobDbContext>>();

            CreateMap<int, TypeModel>()
                .ConvertUsing<PrimaryKeyConverter<TypeModel, JobDbContext>>();

            CreateMap<AddJobRequest, JobModel>()
                .ForMember(m => m.Skills, vm => vm.MapFrom(j => j.SkillIds))
                .ForMember(m => m.Positions, vm => vm.MapFrom(j => j.PositionIds))
                .ForMember(m => m.Categories, vm => vm.MapFrom(j => j.CategoryIds))
                .ForMember(m => m.Types, vm => vm.MapFrom(j => j.TypeIds))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ApplicationRequest, ApplicationModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateJobRequest, JobModel>()
                .ForMember(m => m.Skills, vm => vm.MapFrom(j => j.SkillIds))
                .ForMember(m => m.Positions, vm => vm.MapFrom(j => j.PositionIds))
                .ForMember(m => m.Categories, vm => vm.MapFrom(j => j.CategoryIds))
                .ForMember(m => m.Types, vm => vm.MapFrom(j => j.TypeIds))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<JobModel, JobResponse>()
                .ForMember(vm => vm.ApplicationCount, m => m.MapFrom(j => j.Applications.Count))
                .ForMember(vm => vm.InterestCount, m => m.MapFrom(j => j.Interests.Count))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SkillModel, JobSkillResponse>();
            CreateMap<CategoryModel, JobCategoryResponse>();
            CreateMap<TypeModel, JobTypeResponse>();
            CreateMap<PositionModel, JobPositionResponse>();
            CreateMap<UserModel, JobUserResponse>();
            CreateMap<OrganizationModel, JobOrganizationResponse>();
            CreateMap<InterestModel, InterestResponse>();
            CreateMap<ApplicationModel, ApplicationResponse>();

            CreateMap<JobModel, JobDocument>();
            CreateMap<SkillModel, JobSkillDocument>();
            CreateMap<CategoryModel, JobCategoryDocument>();
            CreateMap<TypeModel, JobTypeDocument>();
            CreateMap<PositionModel, JobPositionDocument>();
            CreateMap<InterestModel, JobInterestDocument>();
            CreateMap<ApplicationModel, JobApplicationDocument>();

            CreateMap<UserModel, UserDocument>();

            CreateMap<Timestamp, DateTime>().ConvertUsing(x => x.ToDateTime());
            CreateMap<JB.gRPC.User.User, UserModel>();
            CreateMap<JB.gRPC.Organization.Organization, OrganizationModel>();
        }
    }


}
