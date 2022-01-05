using HotChocolate;
using HotChocolate.Types;
using JB.Blog.Blog.Models;
using JB.Blog.Services;
using JB.Blog.DTOs.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq.Expressions;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.DTOs;

namespace JB.Blog.GraphQL.Blog
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class BlogQuery
    {
        private readonly IBlogService _blogService;
        private readonly IMapper _mapper;
        private readonly IUserClaimsModel _claims;
        private readonly INotificationService _notificationService;
        public BlogQuery(
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

        [GraphQLName("blogs")]
        public async Task<List<BlogResponse>> Blogs(int? id, [GraphQLName("filter")] ListBlogRequest filterRequest)
        {
            List<BlogResponse> results = new();
            List<BlogModel> blogs = new();
            BlogModel blog = null;
            Status status = new();

            do
            {
                if (id > 0)
                {
                    (status, blog) = await _blogService.GetById(id.Value);
                    if (status.IsSuccess)
                    {
                        results = new List<BlogResponse>()
                        {
                            _mapper.Map<BlogResponse>(blog),
                        };

                        _ = await _blogService.IncresaseView(blog).ConfigureAwait(false);
                    }

                    break;
                }

                Expression<Func<BlogModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<BlogModel>();
                Expression<Func<BlogModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.Id);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 1 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? false;

                if (filterRequest?.IsInterested == true && _claims.Id > 0)
                {
                    filter = filter.And(b => b.Interests.Any(x => x.UserId == _claims.Id));
                }

                (status, blogs) = await _blogService.List(filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                results = blogs.ConvertAll(x => _mapper.Map<BlogResponse>(x));
            }
            while (false);

            return results;
        }

        [GraphQLName("blogs.interested")]
        public async Task<List<BlogResponse>> BlogsInterested([GraphQLName("filter")] ListBlogRequest filterRequest)
        {
            List<BlogResponse> results = new();
            List<BlogModel> blogs = new();
            BlogModel blog = null;
            Status status = new();

            do
            {
                Expression<Func<BlogModel, bool>> filter = filterRequest?.GetFilterExpression() ?? ExpressionHelper.True<BlogModel>();
                Expression<Func<BlogModel, object>> sort = filterRequest?.GetSortExpression() ?? (u => u.Id);
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 1 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? false;

                (status, blogs) = await _blogService.List(filter, sort, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                results = blogs.ConvertAll(x => _mapper.Map<BlogResponse>(x));
            }
            while (false);

            return results;
        }

        [GraphQLName("blogTags")]
        public async Task<List<string>> BlogTags([GraphQLName("filter")] ListRequest filterRequest)
        {
            List<string> results = new();
            List<TagModel> tags = new();
            Status status = new();

            do
            {
                int size = filterRequest?.Size > 0 ? filterRequest.Size.Value : 20;
                int page = filterRequest?.Page > 1 ? filterRequest.Page.Value : 1;
                bool isDescending = filterRequest?.IsDescending ?? false;

                (status, tags) = await _blogService.ListTags(s => true, s => s, size, page, isDescending);
                if (!status.IsSuccess)
                {
                    break;
                }

                results = tags.Select(x => x.Tag).ToList();
            }
            while (false);

            return results;
        }
    }
}
