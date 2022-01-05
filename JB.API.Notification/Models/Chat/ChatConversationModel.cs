using JB.Infrastructure.Models;
using JB.Notification.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Notification.Models.Chat
{
    public class ChatConversationModel : IEntityDate
    {
        public int Id { get; set; }
        
        public int[] UserIds { get; set; }

        [NotMapped]
        public ICollection<UserModel> Users { get; set; }
        
        public virtual ICollection<ChatMessageModel> Messages { get; set; }

        [NotMapped]
        public ChatMessageModel LastMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
