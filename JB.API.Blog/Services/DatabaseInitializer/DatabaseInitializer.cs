using System;
using System.Linq;
using System.Threading.Tasks;
using JB.User.Data;
using JB.User.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using JB.Infrastructure.Helpers;

namespace JB.User.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly CVDbContext _cVDbContext;
        private readonly ProfileDbContext _profileDbContext;

        public DatabaseInitializer(
            IConfiguration configuration,
            CVDbContext cVDbContext,
            ProfileDbContext profileDbContext)
        {
            _configuration = configuration;
            
            _cVDbContext = cVDbContext;
            _profileDbContext = profileDbContext;
        }

        public async Task Initialize()
        {
            //create database schema if none exists
            await _cVDbContext.Database.MigrateAsync();
            await _profileDbContext.Database.MigrateAsync();
        }
    }
}