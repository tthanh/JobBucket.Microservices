using JB.Organization.Models.User;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using JB.Infrastructure.Models;

namespace JB.Organization.Models.Report
{
    public class ReportModel : IEntityDate
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Category { get; set; }
        public bool IsResolved { get; set; }
        public int UserId { get; set; }
        [NotMapped]
        public UserModel User { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int EntityType { get; set; }
        public int EntityId { get; set; }
        public string EntityLink { get; set; }

        [NotMapped]
        public object Entity { get; set; }
    }
}
