using JB.Blog.Models.User;
using JB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Blog.Blog.Models
{
    public class CommentModel : IEntityDate
    {
        [NotMapped]
        public UserModel User { get; set; }

        public int UserId { get; set; }

        public virtual BlogModel Blog { get; set; }

        public int BlogId { get; set; }

        public virtual CommentModel Parent { get; set; }
        public int? ParentId { get; set; }

        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
        
        public virtual ICollection<CommentInterestModel> CommentInterests { get; set; }

        public virtual ICollection<CommentModel> Children { get; set; }

        [NotMapped]
        public bool IsInterested { get; set; }

        [NotMapped]
        public int InterestCount { get; set; }
    }
}
