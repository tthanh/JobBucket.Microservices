using AutoMapper;
using JB.Organization.Models.Organization;
using JB.Organization.Models.User;
using JB.Organization.DTOs.Review.Requests;
using JB.Organization.Models.Review;
using JB.Organization.DTOs.Review.Responses;

namespace JB.Organization.AutoMapper
{
    public class ReviewMapperProfile : Profile
    {
        public ReviewMapperProfile()
        {
            CreateMap<int?, int>().ConvertUsing((src, dest) => { if (src.HasValue) return src.Value; return dest; });

          

            CreateMap<AddReviewRequest, ReviewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

          

            CreateMap<UpdateReviewRequest, ReviewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ReviewModel, ReviewResponse>()
                .ForMember(vm => vm.InterestCount, m => m.MapFrom(j => j.Interests.Count))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ReviewInterestModel, ReviewInterestResponse>();
            CreateMap<UserModel, ReviewUserResponse>();
            CreateMap<OrganizationModel, ReviewOrganizationResponse>();
        }
    }


}
