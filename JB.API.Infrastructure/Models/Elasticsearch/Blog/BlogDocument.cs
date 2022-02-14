using JB.Infrastructure.Elasticsearch.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Infrastructure.Elasticsearch.Blog
{
    public class BlogDocument
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public ICollection<BlogCommentDocument> Comments { get; set; }
        public string[] Tags { get; set; }
        public UserDocument Author { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int InterestCount { get; set; }
        public int CommentCount { get; set; }
        public int Views { get; set; }
    }
}
