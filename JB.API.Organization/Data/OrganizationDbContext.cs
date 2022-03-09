using JB.Organization.Models.Organization;
using JB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JB.Organization.Data
{
    public class OrganizationDbContext : BaseDbContext
    {
        public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options) : base(options)
        {
        }

        public DbSet<OrganizationModel> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Organization");
            
            builder.Entity<OrganizationModel>().Property(x => x.Id).HasIdentityOptions(startValue: 100);

            base.OnModelCreating(builder);
        }
    }
}
