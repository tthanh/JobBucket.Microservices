using JB.Infrastructure.Elasticsearch.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Infrastructure.Elasticsearch.Blog
{
    public class BlogCommentDocument
    {
        public UserDocument User { get; set; }
        public int UserId { get; set; }
        public int BlogId { get; set; }
        public int? ParentId { get; set; }
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<BlogCommentDocument> Children { get; set; }
        public bool IsInterested { get; set; }
        public int InterestCount { get; set; }
    }
}
