using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.DTOs.Chat
{
    public class AddMessageRequest
    {
        [Required]
        public string Content { get; set; }
        public int? Type { get; set; }
        public int? ConversationId { get; set; }
        public int? ReceiverId { get; set; }
    }
}
