using JB.Notification.Models.Notification;
using Microsoft.EntityFrameworkCore;
using JB.Infrastructure.Data;

namespace JB.Notification.Data
{
    public class NotificationDbContext : BaseDbContext
    {
        public DbSet<NotificationModel> Notifications { get; set; }

        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Notification");
            base.OnModelCreating(builder);
        }
    }
}
