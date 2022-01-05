using JB.Infrastructure.Models;
using JB.Notification.Models.User;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Notification.Models.Chat
{
    public class ChatMessageModel : IEntityDate
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        [NotMapped]
        public UserModel Sender { get; set; }
        public int SenderId { get; set; }
        public int ConversationId { get; set; }
        public virtual ChatConversationModel Conversation { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
