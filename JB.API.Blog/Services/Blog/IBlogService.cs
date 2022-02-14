using JB.Blog.Blog.Models;
using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Blog.Services
{
    public interface IBlogService : IServiceBase<BlogModel>
    {
        Task<Status> AddInterestedBlog(int blogId);
        Task<Status> RemoveInterestedBlog(int blogId);
        Task<(Status, CommentModel)> GetCommentById(int id);
        Task<(Status, CommentModel)> AddComment(CommentModel comment);
        Task<(Status, CommentModel)> UpdateComment(CommentModel comment);
        Task<(Status, CommentModel)> DeleteComment(int commentId);
        Task<(Status, CommentModel)> AddInterestedComment(int commentId);
        Task<(Status, CommentModel)> RemoveInterestedComment(int commentId);
        Task<Status> IncresaseView(BlogModel blog);
        Task<(Status, List<TagModel>)> ListTags(Expression<Func<TagModel, bool>> filter, Expression<Func<TagModel, object>> sort, int size, int offset, bool isDescending = false);
    }
}
