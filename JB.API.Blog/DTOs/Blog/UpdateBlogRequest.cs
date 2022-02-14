using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Blog.DTOs.Blog
{
    public class UpdateBlogRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}
