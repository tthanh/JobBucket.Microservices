using JB.Notification.DTOs.User;
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
        public ICollection<UserResponse> Users { get; set; }
        public MessageResponse LastMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
