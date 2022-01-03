using AutoMapper;
using JB.Organization.Models.Organization;
using JB.Organization.Models.User;
using JB.Organization.DTOs.Organization;
using System;
using JB.Infrastructure.Elasticsearch.Organization;

namespace JB.Organization.AutoMapper
{
    public class OrganizationMapperProfile : Profile
    {
        public OrganizationMapperProfile()
        {
            CreateMap<int?, int>().ConvertUsing((src, dest) => { if (src.HasValue) return src.Value; return dest; });

            CreateMap<AddOrganizationRequest, OrganizationModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateOrganizationRequest, OrganizationModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<OrganizationModel, OrganizationResponse>()
                .ForMember(m => m.Rating, vm => vm.MapFrom(j => Math.Round(j.Rating, 2)))
                .ForMember(m => m.RatingBenefit, vm => vm.MapFrom(j => Math.Round(j.RatingBenefit, 2)))
                .ForMember(m => m.RatingCulture, vm => vm.MapFrom(j => Math.Round(j.RatingCulture, 2)))
                .ForMember(m => m.RatingLearning, vm => vm.MapFrom(j => Math.Round(j.RatingLearning, 2)))
                .ForMember(m => m.RatingWorkspace, vm => vm.MapFrom(j => Math.Round(j.RatingWorkspace, 2)))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<OrganizationModel, OrganizationDetailResponse>()
                .ForMember(m => m.Rating, vm => vm.MapFrom(j => Math.Round(j.Rating, 2)))
                .ForMember(m => m.RatingBenefit, vm => vm.MapFrom(j => Math.Round(j.RatingBenefit, 2)))
                .ForMember(m => m.RatingCulture, vm => vm.MapFrom(j => Math.Round(j.RatingCulture, 2)))
                .ForMember(m => m.RatingLearning, vm => vm.MapFrom(j => Math.Round(j.RatingLearning, 2)))
                .ForMember(m => m.RatingWorkspace, vm => vm.MapFrom(j => Math.Round(j.RatingWorkspace, 2)))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserModel, OrganizationUserResponse>();
            CreateMap<UserModel, AddEmployerResponse>();

            CreateMap<OrganizationModel, OrganizationDocument>();
        }
    }


}
