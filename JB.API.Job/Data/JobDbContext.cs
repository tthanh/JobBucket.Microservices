using JB.API.Data;
using JB.Job.Models.Job;
using Microsoft.EntityFrameworkCore;

namespace JB.Job.Data
{
    public class JobDbContext : BaseDbContext
    {
        public JobDbContext(DbContextOptions<JobDbContext> options) : base(options)
        {
        }

        public DbSet<JobModel> Jobs { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<InterestModel> Interests { get; set; }
        public DbSet<ApplicationModel> Application { get; set; }
        public DbSet<PositionModel> Positions { get; set; }
        public DbSet<SkillModel> Skillls { get; set; }
        public DbSet<TypeModel> Types { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Job");

            builder.Entity<JobModel>().Property(x => x.Id).HasIdentityOptions(startValue: 100);
            builder.Entity<SkillModel>().Property(x => x.Id).HasIdentityOptions(startValue: 100);
            builder.Entity<CategoryModel>().Property(x => x.Id).HasIdentityOptions(startValue: 100);
            builder.Entity<PositionModel>().Property(x => x.Id).HasIdentityOptions(startValue: 100);
            builder.Entity<TypeModel>().Property(x => x.Id).HasIdentityOptions(startValue: 100);

            builder.Entity<ApplicationModel>().HasKey(table => new
            {
                table.JobId,
                table.UserId
            });

            builder.Entity<InterestModel>().HasKey(table => new
            {
                table.JobId,
                table.UserId
            });

            base.OnModelCreating(builder);
        }
    }
}
