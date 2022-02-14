using JB.Infrastructure.Data;
using JB.User.Models.CV;
using Microsoft.EntityFrameworkCore;

namespace JB.User.Data
{
    public class CVDbContext : BaseDbContext
    {
        public DbSet<CVModel> CVs { get; set; }

        public CVDbContext(DbContextOptions<CVDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("CV");
            
            base.OnModelCreating(builder);
        }
    }
}
