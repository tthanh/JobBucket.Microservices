using JB.Blog.Blog.Models;
using JB.Blog.Models.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JB.Blog.Blog.Models
{
    public class CommentInterestModel
    {
        public int UserId { get; set; }

        public virtual CommentModel Comment { get; set; }
        public int CommentId { get; set; }
    }
}
