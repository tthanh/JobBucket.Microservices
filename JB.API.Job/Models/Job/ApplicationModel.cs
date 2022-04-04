using JB.Infrastructure.Models;
using JB.Job.Models.User;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Job.Models.Job
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
        public string [] Attachments { get; set; }
        public string Introdution { get; set; }
    }
}
