using JB.Infrastructure.Data;
using JB.Notification.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace JB.Notification.Data
{
    public class ChatDbContext : BaseDbContext
    {
        public DbSet<ChatMessageModel> Messages { get; set; }
        public DbSet<ChatConversationModel> Conversations { get; set; }

        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Chat");

            base.OnModelCreating(builder);
        }
    }
}
