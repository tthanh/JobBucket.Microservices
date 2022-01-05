using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Blog.DTOs.Blog
{
    public class AddBlogCommentRequest
    {
        [Required]
        [MinLength(50)]
        [MaxLength(1000)]
        public string Content { get; set; }

        public int? ParentId { get; set; }
        public int BlogId { get; set; }
    }
}
