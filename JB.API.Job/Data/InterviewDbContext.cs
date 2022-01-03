using JB.API.Data;
using JB.Job.Models.Interview;
using Microsoft.EntityFrameworkCore;

namespace JB.Job.Data
{
    public class InterviewDbContext : BaseDbContext
    {
        public DbSet<InterviewModel> Interviews { get; set; }

        public InterviewDbContext(DbContextOptions<InterviewDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Interview");

            base.OnModelCreating(builder);
        }
    }
}
