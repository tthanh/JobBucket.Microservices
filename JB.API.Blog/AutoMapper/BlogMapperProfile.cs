using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using JB.Blog.Blog.Models;
using JB.Blog.DTOs.Blog;
using JB.Blog.Models.User;
using JB.Infrastructure.Elasticsearch.Blog;
using JB.Infrastructure.Elasticsearch.User;
using System;

namespace JB.Blog.AutoMapper
{
    public class BlogMapperProfile : Profile
    {
        public BlogMapperProfile()
        {
            CreateMap<AddBlogRequest, BlogModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateBlogRequest, BlogModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AddBlogCommentRequest, CommentModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateBlogCommentRequest, CommentModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserModel, BlogUserResponse>();
            CreateMap<CommentModel, BlogCommentResponse>();
            CreateMap<BlogModel, BlogResponse>();

            CreateMap<CommentModel, BlogCommentDocument>();
            CreateMap<BlogModel, BlogDocument>();

            CreateMap<Timestamp, DateTime>().ConvertUsing(x => x.ToDateTime());
            CreateMap<JB.gRPC.User.User, UserModel>();

            CreateMap<CommentModel, BlogCommentDocument>();
            CreateMap<BlogModel, BlogDocument>();
            CreateMap<UserModel, UserDocument>();
        }
    }


}
