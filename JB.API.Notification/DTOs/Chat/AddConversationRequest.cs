using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.DTOs.Chat
{
    public class AddConversationRequest
    {
        public int ReceiverId { get; set; }
    }
}
