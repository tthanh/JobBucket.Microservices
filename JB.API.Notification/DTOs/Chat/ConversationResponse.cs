using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.DTOs.Chat
{
    public class ConversationResponse
    {
        public int Id { get; set; }
        public int[] UserIds { get; set; }
        public ICollection<ChatUserResponse> Users { get; set; }
        public MessageResponse LastMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
