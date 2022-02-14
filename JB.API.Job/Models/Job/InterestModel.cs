using JB.Job.Models.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Job.Models.Job
{
    public class InterestModel
    {
        [NotMapped]
        public UserModel User { get; set; }
        public int UserId { get; set; }
        public virtual JobModel Job { get; set; }
        public int JobId { get; set; }
    }
}
