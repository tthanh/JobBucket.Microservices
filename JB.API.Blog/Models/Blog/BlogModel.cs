using JB.Blog.Models.User;
using JB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Blog.Blog.Models
{
    public class BlogModel : IEntityDate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public virtual ICollection<BlogInterestModel> Interests { get; set; }
        public virtual ICollection<CommentModel> Comments { get; set; }
        public string[] Tags { get; set; }

        [NotMapped]
        public UserModel Author { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public DateTime UpdatedDate { get; set; }

        [NotMapped]
        public bool IsInterested { get; set; }

        [NotMapped]
        public int InterestCount { get; set; }

        [NotMapped]
        public int CommentCount { get; set; }

        public int Views { get; set; }
    }
}
