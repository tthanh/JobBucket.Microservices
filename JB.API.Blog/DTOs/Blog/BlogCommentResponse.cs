using JB.Blog.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Blog.DTOs.Blog
{
    public class BlogCommentResponse
    {
        public BlogUserResponse User { get; set; }
        public int UserId { get; set; }
        public int BlogId { get; set; }
        public int? ParentId { get; set; }
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<BlogCommentResponse> Children { get; set; }
        public bool IsInterested { get; set; }
        public int InterestCount { get; set; }
    }
}
