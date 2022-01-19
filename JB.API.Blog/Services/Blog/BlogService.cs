using AutoMapper;
using JB.Blog.Blog.Models;
using JB.Blog.Data;
using JB.Blog.Models.User;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Elasticsearch.Blog;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JB.Blog.Services
{
    public class BlogService : IBlogService
    {
        private const string DELETED_COMMENT_LABEL = "[Comment deleted]";
        private readonly BlogDbContext _blogDbContext;
        private readonly IUserManagementService _userManagementService;
        private readonly IMapper _mapper;
        private readonly ILogger<BlogService> _logger;
        private readonly IUserClaimsModel _claims;
        private readonly INotificationService _notificationService;
        private readonly Nest.IElasticClient _elasticClient;

        private const string splitRegex = "[^a-zA-Z0-9]";

        public BlogService(
            BlogDbContext blogDbContext,
            IUserManagementService userManagementService,
            IMapper mapper,
            ILogger<BlogService> logger,
            IUserClaimsModel claims,
            INotificationService notificationService,
            Nest.IElasticClient elasticClient
            )
        {
            _blogDbContext = blogDbContext;
            _userManagementService = userManagementService;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _notificationService = notificationService;
            _elasticClient = elasticClient;
        }

        public async Task<Status> Add(BlogModel entity)
        {
            Status result = new Status();

            do
            {
                try
                {
                    if (entity == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }
                    
                    entity.AuthorId = _claims.Id;
                    await _blogDbContext.Blogs.AddAsync(entity);
                    await _blogDbContext.SaveChangesAsync();

                    await UpdateBlogTag(entity);

                    (var getUserStatus, var author) = await _userManagementService.GetUser(entity.AuthorId);
                    if (getUserStatus.IsSuccess)
                    {
                        entity.Author = author;
                    }

                    await AddDocument(entity);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }
        public async Task<(Status, long)> Count(Expression<Func<BlogModel, bool>> predicate)
        {
            Status result = new Status();
            long blogsCount = 0;
            do
            {
                try
                {
                    if (predicate == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    blogsCount = await _blogDbContext.Blogs.Where(predicate)?.CountAsync();

                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return (result, blogsCount);
        }
        public async Task<Status> Delete(int id)
        {
            Status result = new();
            BlogModel blog = null;

            do
            {
                try
                {
                    if (id <= 0)
                    {
                        result.ErrorCode = ErrorCode.BlogNotExist;
                        break;
                    }

                    blog = await _blogDbContext.Blogs.FirstOrDefaultAsync(x => x.Id == id);

                    if (blog == null)
                    {
                        result.ErrorCode = ErrorCode.BlogNotExist;
                        break;
                    }

                    if (_claims.RoleId != (int)RoleType.Admin && blog.AuthorId != _claims.Id)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _blogDbContext.Blogs.Remove(blog);
                    _blogDbContext.SaveChanges();

                    await DeleteDocument(blog.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }
        public async Task<(Status, List<BlogModel>)> List(Expression<Func<BlogModel, bool>> filter, Expression<Func<BlogModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status status = new Status();
            var blogs = new List<BlogModel>();

            do
            {
                try
                {
                    var blogQuery = _blogDbContext.Blogs.Where(filter);
                    blogQuery = isDescending ? blogQuery.OrderByDescending(sort) : blogQuery.OrderBy(sort);
                    blogs = await blogQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (blogs == null)
                    {
                        break;
                    }

                    foreach (var blog in blogs)
                    {
                        (var getUserStatus, var author) = await _userManagementService.GetUser(blog.AuthorId);
                        if (getUserStatus.IsSuccess)
                        {
                            blog.Author = author;
                        }

                        foreach (var c in blog.Comments)
                        {
                            (var getCreatorStatus, var creator) = await _userManagementService.GetUser(c.UserId);
                            if (status.IsSuccess)
                            {
                                c.User = creator;
                            }
                        }

                        blog.CommentCount = blog.Comments?.Count ?? 0;
                        blog.InterestCount = blog.Interests?.Count ?? 0;
                        blog.IsInterested = blog.Interests?.Any(x => x.UserId == _claims.Id) ?? false;

                        foreach (var c in blog.Comments)
                        {
                            c.InterestCount = c.CommentInterests?.Count ?? 0;
                            c.IsInterested = c.CommentInterests?.Any(x => x.UserId == _claims.Id) ?? false;
                            if (c.Children == null)
                            {
                                c.Children = new List<CommentModel>();
                            }
                        }

                        _blogDbContext.Entry(blog).State = EntityState.Detached;
                        blog.Comments = blog.Comments.Where(c => c.ParentId == null).ToList();
                        blog.Comments = blog.Comments.OrderByDescending(x => x.CreatedDate).ToList();
                        
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    status.ErrorCode = ErrorCode.Unknown;
                }

            }
            while (false);

            return (status, blogs);
        }
        public async Task<(Status, BlogModel)> GetById(int id)
        {
            Status status = new Status();
            BlogModel blog = null;
            UserModel author = null;

            do
            {
                try
                {
                    blog = await _blogDbContext.Blogs.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (blog == null)
                    {
                        break;
                    }

                    (status, author) = await _userManagementService.GetUser(blog.AuthorId);
                    if (!status.IsSuccess)
                    {
                        break;
                    }

                    blog.Author = author;
                    blog.CommentCount = blog.Comments?.Count ?? 0;

                    foreach (var c in blog.Comments)
                    {
                        c.InterestCount = c.CommentInterests?.Count ?? 0;
                        c.IsInterested = c.CommentInterests?.Any(x => x.UserId == _claims.Id) ?? false;
                        (var getCreatorStatus, var creator) = await _userManagementService.GetUser(c.UserId);
                        if (getCreatorStatus.IsSuccess)
                        {
                            c.User = creator;
                        }
                    }

                    blog.InterestCount = blog.Interests?.Count ?? 0;
                    blog.IsInterested = blog.Interests?.Any(x => x.UserId == _claims.Id) ?? false;
                    _blogDbContext.Entry(blog).State = EntityState.Detached;
                    blog.Comments = blog.Comments.Where(c => c.ParentId == null).ToList();
                    blog.Comments = blog.Comments.OrderByDescending(x => x.CreatedDate).ToList();
                    foreach (var c in blog.Comments)
                    {
                        if (c.Children == null)
                        {
                            c.Children = new List<CommentModel>();
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    status.ErrorCode = ErrorCode.Unknown;
                }

            }
            while (false);

            return (status, blog);
        }
        public async Task<Status> Update(BlogModel entity)
        {
            Status result = new();

            do
            {
                try
                {
                    if (entity == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    if (_claims?.Id <= 0 || entity.AuthorId != _claims.Id)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _blogDbContext.Update(entity);
                    await _blogDbContext.SaveChangesAsync();

                    await UpdateBlogTag(entity);

                    (var getUserStatus, var author) = await _userManagementService.GetUser(entity.AuthorId);
                    if (getUserStatus.IsSuccess)
                    {
                        entity.Author = author;
                    }

                    await UpdateDocument(entity);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }
        public async Task<Status> AddInterestedBlog(int blogId)
        {
            Status result = new();

            do
            {
                try
                {
                    if (blogId <= 0 || _claims.Id <= 0)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    var interestFromDb = await _blogDbContext.Interests.FirstOrDefaultAsync(i => i.BlogId == blogId && i.UserId == _claims.Id);
                    if (interestFromDb != null)
                    {
                        result.ErrorCode = ErrorCode.BlogAlreadyLiked;
                        break;
                    }

                    await _blogDbContext.Interests.AddAsync(new BlogInterestModel { BlogId = blogId, UserId = _claims.Id });
                    await _blogDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }
        public async Task<Status> RemoveInterestedBlog(int blogId)
        {
            Status result = new Status();
            int userId = _claims.Id;

            do
            {
                if (blogId <= 0 || userId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                try
                {
                    var interestFromDb = await _blogDbContext.Interests.FirstOrDefaultAsync(i => i.BlogId == blogId && i.UserId == _claims.Id);
                    if (interestFromDb == null)
                    {
                        result.ErrorCode = ErrorCode.BlogNotLiked;
                        break;
                    }

                    _blogDbContext.Interests.Remove(interestFromDb);
                    await _blogDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return result;
        }
        public async Task<(Status, CommentModel)> AddComment(CommentModel comment)
        {
            Status result = new Status();

            do
            {
                if (comment == null || comment.BlogId <= 0 || string.IsNullOrEmpty(comment.Content))
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                comment.UserId = _claims.Id;
                if (comment.UserId <= 0)
                {
                    result.ErrorCode = ErrorCode.NoPrivilege;
                    break;
                }

                try
                {
                    await _blogDbContext.Comments.AddAsync(comment);
                    await _blogDbContext.SaveChangesAsync();

                    (result, comment) = await GetCommentById(comment.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return (result, comment);
        }
        public async Task<(Status, CommentModel)> UpdateComment(CommentModel comment)
        {
            Status result = new Status();

            do
            {
                if (comment == null)
                {
                    result.ErrorCode = ErrorCode.JobNull;
                    break;
                }

                try
                {
                    if (_claims?.Id <= 0 || comment.UserId != _claims.Id)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _blogDbContext.Update(comment);

                    await _blogDbContext.SaveChangesAsync();

                    (result, comment) = await GetCommentById(comment.Id);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, comment);
        }
        public async Task<(Status, CommentModel)> DeleteComment(int commentId)
        {
            Status result = new Status();
            CommentModel comment = null;

            do
            {
                if (commentId <= 0)
                {
                    result.ErrorCode = ErrorCode.JobNull;
                    break;
                }

                try
                {
                    comment = _blogDbContext.Comments.FirstOrDefault(x => x.Id == commentId);
                    if (comment == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }

                    comment.Content = DELETED_COMMENT_LABEL;
                    await _blogDbContext.SaveChangesAsync();

                    (result, comment) = await GetCommentById(comment.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return (result, comment);
        }
        public async Task<(Status, CommentModel)> AddInterestedComment(int commentId)
        {
            Status result = new Status();
            CommentModel comment = null;
            CommentInterestModel commentInterest = null;

            do
            {
                if (commentId <= 0 || _claims.Id <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                try
                {
                    commentInterest = await _blogDbContext.CommentInterests.FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == _claims.Id);
                    if (commentInterest == null)
                    {
                        await _blogDbContext.CommentInterests.AddAsync(new CommentInterestModel { CommentId = commentId, UserId = _claims.Id });
                        await _blogDbContext.SaveChangesAsync();
                    }

                    (result, comment) = await GetCommentById(commentId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return (result, comment);
        }
        public async Task<(Status, CommentModel)> RemoveInterestedComment(int commentId)
        {
            Status result = new Status();
            CommentModel comment = null;
            CommentInterestModel commentInterest = null;

            do
            {
                if (commentId <= 0 || _claims.Id <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                try
                {
                    commentInterest = await _blogDbContext.CommentInterests.FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == _claims.Id);
                    if (commentInterest != null)
                    {
                        _blogDbContext.CommentInterests.Remove(commentInterest);
                        await _blogDbContext.SaveChangesAsync();
                    }

                    (result, comment) = await GetCommentById(commentId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    result.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return (result, comment);
        }
        public async Task<(Status, CommentModel)> GetCommentById(int id)
        {
            Status status = new Status();
            CommentModel comment = null;

            do
            {
                try
                {
                    comment = await _blogDbContext.Comments.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (comment == null)
                    {
                        break;
                    }

                    (var getCreatorStatus, var creator) = await _userManagementService.GetUser(comment.UserId);
                    if (getCreatorStatus.IsSuccess)
                    {
                        comment.User = creator;
                    }

                    comment.InterestCount = comment.CommentInterests?.Count ?? 0;
                    comment.IsInterested = comment.CommentInterests?.Any(x => x.UserId == _claims.Id) ?? false;
                    if (comment.Children == null)
                    {
                        comment.Children = new List<CommentModel>();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    status.ErrorCode = ErrorCode.Unknown;
                }

            }
            while (false);

            return (status, comment);
        }

        public async Task<Status> IncresaseView(BlogModel blog)
        {
            Status status = new Status();

            do
            {
                try
                {
                    if (blog == null)
                    {
                        break;
                    }
                        

                    blog.Views++;

                    _ = await _blogDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    status.ErrorCode = ErrorCode.Unknown;
                }

            }
            while (false);

            return status;
        }
        public async Task<(Status, List<TagModel>)> ListTags(Expression<Func<TagModel, bool>> filter, Expression<Func<TagModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status status = new Status();
            var tags = new List<TagModel>();

            do
            {
                try
                {
                    var tagQuery = _blogDbContext.Tags.Where(filter);
                    tagQuery = isDescending ? tagQuery.OrderByDescending(sort) : tagQuery.OrderBy(sort);
                    tags = await tagQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    status.ErrorCode = ErrorCode.Unknown;
                }
            }
            while (false);

            return (status, tags);
        }

        private async Task AddTag(params string[] values)
        {
            do
            {
                try
                {
                    var uninserted = values.Where(x => !_blogDbContext.Tags.Any(t => t.Tag == x));

                    await _blogDbContext.Tags.AddRangeAsync(uninserted.Select(x => new TagModel
                    {
                        Tag = x
                    }));
                    await _blogDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);
        }

        private async Task DeleteTag(params string[] values)
        {
            do
            {
                try
                {
                    var inserted = _blogDbContext.Tags.Where(x => !values.Contains(x.Tag));

                    _blogDbContext.Tags.RemoveRange(inserted);
                    await _blogDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);
        }

        private string[] GetFormattedTags(params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                return values;
            }

            List<string> resut = new List<string>();

            foreach (var val in values)
            {
                var splitted = Regex.Split(val, splitRegex)?.Select(x => x.ToLower()).Where(x => !string.IsNullOrEmpty(x));
                if (splitted != null)
                {
                    resut.AddRange(splitted);
                }
            }

            return resut.ToArray();
        }

        private async Task UpdateBlogTag(BlogModel blog)
        {
            if (blog?.Tags != null)
            {
                var formattedTags = GetFormattedTags(blog.Tags);

                await AddTag(formattedTags);
            }
        }

        private async Task AddDocument(BlogModel blog)
        {
            BlogDocument doc = _mapper.Map<BlogDocument>(blog);
            await _elasticClient.IndexAsync(doc, r => r.Index("blog"));
        }

        private async Task UpdateDocument(BlogModel blog)
        {
            BlogDocument doc = _mapper.Map<BlogDocument>(blog);
            await _elasticClient.UpdateAsync<BlogDocument>(blog.Id, u => u.Index("blog").Doc(doc));
        }

        private async Task DeleteDocument(int id)
        {
            await _elasticClient.DeleteAsync<BlogModel>(id, r => r.Index("blog"));
        }
    }
}
