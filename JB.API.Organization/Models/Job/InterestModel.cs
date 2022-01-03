using JB.Organization.Models.User;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Organization.Models.Job
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
