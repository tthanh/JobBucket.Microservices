using JB.Blog.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Blog.DTOs.Blog
{
    public class BlogResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public ICollection<BlogCommentResponse> Comments { get; set; }
        public string[] Tags { get; set; }
        public BlogUserResponse Author { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsInterested { get; set; }
        public int InterestCount { get; set; }
        public int CommentCount { get; set; }
        public int Views { get; set; }
    }
}
