using JB.Notification.Models.Organization;
using JB.Notification.Models.User;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using JB.Infrastructure.Models;

namespace JB.Notification.Models.Notification
{
    public class NotificationModel : IEntityDate
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int Type { get; set; }

        public int Level { get; set; }

        public int SenderId { get; set; }

        [NotMapped]
        public UserModel Sender { get; set; }

        public int OrganizationId { get; set; }

        [NotMapped]
        public OrganizationModel Organization { get; set; }

        public int ReceiverId { get; set; }

        public bool SeenByUser { get; set; }

        public string Message { get; set; }

        public string Data { get; set; }
    }
}
