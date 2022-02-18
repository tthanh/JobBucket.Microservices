using System;

namespace JB.Infrastructure.DTOs.Subscriptions
{
    public class SubscriptionsNotificationResponse
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Type { get; set; }
        public int Level { get; set; }
        public int SenderId { get; set; }
        public SubscriptionsUserResponse Sender { get; set; }
        public int OrganizationId { get; set; }
        public SubscriptionsOrganizationResponse Organization { get; set; }
        public int ReceiverId { get; set; }
        public bool SeenByUser { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
}
