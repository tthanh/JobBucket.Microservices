using JB.Organization.Models.Review;
using JB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JB.Organization.Data
{
    public class ReviewDbContext : BaseDbContext
    {
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<ReviewInterestModel> ReviewInterests { get; set; }

        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Review");

            builder.Entity<ReviewInterestModel>().HasKey(table => new
            {
                table.UserId,
                table.ReviewId
            });

            base.OnModelCreating(builder);
        }
    }
}
