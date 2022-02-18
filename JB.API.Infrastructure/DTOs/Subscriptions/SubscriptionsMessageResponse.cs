using System;

namespace JB.Infrastructure.DTOs.Subscriptions
{
    public class SubscriptionsMessageResponse
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public SubscriptionsUserResponse Sender { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int ConversationId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
