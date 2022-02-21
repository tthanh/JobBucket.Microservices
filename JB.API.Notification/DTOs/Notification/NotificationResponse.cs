using System;
using JB.Notification.Models.User;
using JB.Notification.Models.Organization;
using JB.Notification.DTOs.Organization;
using JB.Notification.DTOs.User;

namespace JB.Notification.DTOs.Notification
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
        public int SenderId { get; set; }
        public NotificationUserResponse Sender { get; set; }
        public int OrganizationId { get; set; }
        public NotificationOrganizationResponse Organization { get; set; }
        public int ReceiverId { get; set; }
        public bool SeenByUser { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
}
