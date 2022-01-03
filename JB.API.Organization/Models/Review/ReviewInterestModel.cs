using JB.Organization.Models.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Organization.Models.Review
{
    public class ReviewInterestModel
    {
        [NotMapped]
        public UserModel User { get; set; }
        public int UserId { get; set; }
        public virtual ReviewModel Review { get; set; }
        public int ReviewId { get; set; }
    }
}
