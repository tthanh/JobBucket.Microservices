namespace JB.Blog.Blog.Models
{
    public class BlogInterestModel
    {
        public int UserId { get; set; }

        public virtual BlogModel Blog { get; set; }
        public int BlogId { get; set; }
    }
}
