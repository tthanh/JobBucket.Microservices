using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.DTOs.Chat
{
    public class ChatUserResponse
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }
    }
}
