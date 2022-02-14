using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.DTOs.Review.Responses
{
    public class ReviewInterestResponse
    {
        public ReviewUserResponse User { get; set; }
        public int UserId { get; set; }
        public virtual ReviewResponse Review { get; set; }
        public int ReviewId { get; set; }
    }
}
