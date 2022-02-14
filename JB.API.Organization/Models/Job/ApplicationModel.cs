using JB.Organization.Models.User;
using JB.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Organization.Models.Job
{
    public class ApplicationModel : IEntityDate
    {
        [NotMapped]
        public UserModel User { get; set; }
        public int UserId { get; set; }
        public virtual JobModel Job { get; set; }
        public int JobId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Status { get; set; }
        public int CVId { get; set; }
        public string CVPDFUrl { get; set; }
    }
}
