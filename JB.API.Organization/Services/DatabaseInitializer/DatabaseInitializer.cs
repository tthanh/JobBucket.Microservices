using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JB.Organization.Data;
using JB.Organization.Models;
using JB.Organization.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JB.Organization.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly ReviewDbContext _reviewDbContext;

        public DatabaseInitializer(
            IConfiguration configuration,
            OrganizationDbContext organizationDbContext,
            ReviewDbContext reviewDbContext)
        {
            _configuration = configuration;
            
            _organizationDbContext = organizationDbContext;
            _reviewDbContext = reviewDbContext;
        }

        public async Task Initialize()
        {
            //create database schema if none exists
            await _organizationDbContext.Database.MigrateAsync();
            await _reviewDbContext.Database.MigrateAsync();
        }
    }
}