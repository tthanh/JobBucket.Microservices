using System;
using System.Linq;
using System.Threading.Tasks;
using JB.Notification.Data;
using JB.Notification.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using JB.Infrastructure.Helpers;

namespace JB.Notification.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly ChatDbContext _chatDbContext;
        private readonly NotificationDbContext _notificationDbContext;

        public DatabaseInitializer(
            IConfiguration configuration,
            ChatDbContext chatDbContext,
            NotificationDbContext notificationDbContext)
        {
            _configuration = configuration;

            _chatDbContext = chatDbContext;
            _notificationDbContext = notificationDbContext;
        }

        public async Task Initialize()
        {
            //create database schema if none exists
            await _chatDbContext.Database.MigrateAsync();
            await _notificationDbContext.Database.MigrateAsync();
        }
    }
}