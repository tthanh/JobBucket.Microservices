

using JB.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JB.API.Data
{
    public abstract class BaseDbContext : DbContext
    {
        public IServiceProvider ServiceProvider;

        protected BaseDbContext() : base()
        {
        }
        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        protected BaseDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options)
        {
            ServiceProvider = serviceProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetEnityDate();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            SetEnityDate();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetEnityDate();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetEnityDate();
            return base.SaveChangesAsync(cancellationToken);
        }

        public void SetEnityDate()
        {
            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is IEntityDate entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.CreatedDate = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            entity.UpdatedDate = DateTime.UtcNow;
                            break;
                    }
                }
            }
        }
    }
}
