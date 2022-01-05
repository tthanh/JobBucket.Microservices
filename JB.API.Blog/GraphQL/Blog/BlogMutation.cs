using JB.Blog.Services;
using JB.Blog.DTOs.Blog;
using System.Threading.Tasks;
using HotChocolate.Resolvers;
using AutoMapper;
using JB.Blog.Blog.Models;
using HotChocolate;
using JB.Infrastructure.Models;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models.Authentication;

namespace JB.Blog.GraphQL.Blog
{
    public class BlogMutation
    {
        private readonly IBlogService _blogService;
        private readonly IMapper _mapper;
        private readonly IUserClaimsModel _claims;
        private readonly INotificationService _notificationService;
        public BlogMutation(
            IBlogService blogService,
            IMapper mapper,
            IUserClaimsModel claims,
            INotificationService notificationService)
        {
            _claims = claims;
            _mapper = mapper;
            _blogService = blogService;
            _notificationService = notificationService;
        }

        public async Task<BlogResponse> Add(IResolverContext context, [GraphQLName("blog")] AddBlogRequest blogRequest)
        {
            Status status = new();
            BlogResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(blogRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                var blog = _mapper.Map<BlogModel>(blogRequest);
                if (blog == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _blogService.Add(blog);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<BlogResponse>(blog);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<BlogResponse> Update(IResolverContext context, [GraphQLName("blog")] UpdateBlogRequest blogRequest)
        {
            Status status = new();
            BlogModel blog = null;
            BlogResponse result = null;

            do
            {
                if (!PropertyHelper.TryValidateObject(blogRequest, out var errors))
                {
                    status.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, blog) = await _blogService.GetById(blogRequest.Id);
                if (!status.IsSuccess)
                {
                    break;
                }

                blog = _mapper.Map(blogRequest, blog);
                if (blog == null)
                {
                    status.ErrorCode = ErrorCode.InvalidData;
                    break;
                }

                status = await _blogService.Update(blog);
                if (!status.IsSuccess)
                {
                    break;
                }

                result = _mapper.Map<BlogResponse>(blog);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return result;
        }
        public async Task<BlogResponse> Delete(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _blogService.Delete(id);
                if (!status.IsSuccess)
                {
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<BlogResponse> AddInterested(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _blogService.AddInterestedBlog(id);
                if (!status.IsSuccess)
                {
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<BlogResponse> RemoveInterested(IResolverContext context, int id)
        {
            Status status = new();

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                status = await _blogService.RemoveInterestedBlog(id);
                if (!status.IsSuccess)
                {
                    break;
                }
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return null;
        }
        public async Task<BlogCommentResponse> AddComment(IResolverContext context, [GraphQLName("comment")] AddBlogCommentRequest commentRequest)
        {
            Status status = new();
            BlogCommentResponse commentResponse = null;

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                var comment = _mapper.Map<CommentModel>(commentRequest);
                (status, comment) = await _blogService.AddComment(comment);
                if (!status.IsSuccess)
                {
                    break;
                }

                commentResponse = _mapper.Map<BlogCommentResponse>(comment);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return commentResponse;
        }
        public async Task<BlogCommentResponse> UpdateComment(IResolverContext context, [GraphQLName("comment")] UpdateBlogCommentRequest commentRequest)
        {
            Status status = new();
            CommentModel comment = null;
            BlogCommentResponse commentResponse = null;

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, comment) = await _blogService.GetCommentById(commentRequest.Id);
                if (!status.IsSuccess)
                {
                    break;
                }

                comment = _mapper.Map(commentRequest, comment);
                (status, comment) = await _blogService.UpdateComment(comment);
                commentResponse = _mapper.Map<BlogCommentResponse>(comment);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return commentResponse;
        }
        public async Task<BlogCommentResponse> DeleteComment(IResolverContext context, int id)
        {
            Status status = new();
            BlogCommentResponse commentResponse = null;
            CommentModel comment = null;

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, comment) = await _blogService.DeleteComment(id);
                if (!status.IsSuccess)
                {
                    break;
                }

                commentResponse = _mapper.Map<BlogCommentResponse>(comment);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return commentResponse;
        }
        public async Task<BlogCommentResponse> AddInterestedComment(IResolverContext context, int id)
        {
            Status status = new();
            BlogCommentResponse commentResponse = null;
            CommentModel comment = null;

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, comment) = await _blogService.AddInterestedComment(id);
                if (!status.IsSuccess)
                {
                    break;
                }

                commentResponse = _mapper.Map<BlogCommentResponse>(comment);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return commentResponse;
        }
        public async Task<BlogCommentResponse> RemoveInterestedComment(IResolverContext context, int id)
        {
            Status status = new();
            BlogCommentResponse commentResponse = null;
            CommentModel comment = null;

            do
            {
                if (_claims.Id <= 0)
                {
                    status.ErrorCode = ErrorCode.Unauthorized;
                    break;
                }

                (status, comment) = await _blogService.RemoveInterestedComment(id);
                if (!status.IsSuccess)
                {
                    break;
                }

                commentResponse = _mapper.Map<BlogCommentResponse>(comment);
            }
            while (false);

            if (!status.IsSuccess)
            {
                context.ReportError(status.Message);
            }

            return commentResponse;
        }
    }
}
