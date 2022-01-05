using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.DTOs.Chat
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public ChatUserResponse Sender { get; set; }
        public int SenderId { get; set; }
        public int ConversationId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
