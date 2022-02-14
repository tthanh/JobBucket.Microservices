using JB.Blog.Blog.Models;
using JB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace JB.Blog.Data
{
    public class BlogDbContext : BaseDbContext
    {
        public DbSet<BlogModel> Blogs { get; set; }
        public DbSet<BlogInterestModel> Interests { get; set; }
        public DbSet<CommentInterestModel> CommentInterests { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<TagModel> Tags { get; set; }

        public BlogDbContext(DbContextOptions<BlogDbContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Blog");

            builder.Entity<BlogInterestModel>().HasKey(table => new {
                table.BlogId,
                table.UserId
            });

            builder.Entity<CommentInterestModel>().HasKey(table => new {
                table.CommentId,
                table.UserId
            });

            builder.Entity<CommentModel>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }
    }
}
