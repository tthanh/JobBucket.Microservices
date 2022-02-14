using System.Threading.Tasks;
using JB.Blog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JB.Blog.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly BlogDbContext _blogDbContext;

        public DatabaseInitializer(
            IConfiguration configuration,
            BlogDbContext blogDbContext)
        {
            _configuration = configuration;
            _blogDbContext = blogDbContext;
        }

        public async Task Initialize()
        {
            //create database schema if none exists
            await _blogDbContext.Database.MigrateAsync();
        }
    }
}