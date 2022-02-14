using JB.Infrastructure.Data;
using JB.User.Models.Profile;
using Microsoft.EntityFrameworkCore;
using System;

namespace JB.User.Data
{
    public class ProfileDbContext : BaseDbContext
    {
        public DbSet<UserProfileModel> Profiles { get; set; }

        public ProfileDbContext(DbContextOptions<ProfileDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Profile");

            base.OnModelCreating(builder);
        }
    }
}
