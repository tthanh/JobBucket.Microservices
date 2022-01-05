using System;
using System.Linq;
using System.Threading.Tasks;
using JB.Authentication.Data;
using JB.Authentication.Models.User;
using JB.Authentication.Services;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JB.Authentication.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly AuthenticationDbContext _authenticationDbContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public DatabaseInitializer(
            IConfiguration configuration,
            AuthenticationDbContext authenticationDbContext,
            UserManager<UserModel> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _configuration = configuration;

            _authenticationDbContext = authenticationDbContext;

            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            //create database schema if none exists
            await _authenticationDbContext.Database.MigrateAsync();

            if (await _authenticationDbContext.Roles.CountAsync() == 0)
            {
                foreach (var r in Enum.GetValues(typeof(RoleType)).Cast<RoleType>())
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>
                    {
                        Id = (int)r,
                        Name = EnumHelper.GetDescriptionFromEnumValue(r),
                    });
                }

                var admin = new UserModel
                {
                    Email = _configuration["APIUser:Admin:Email"],
                    UserName = _configuration["APIUser:Admin:UserName"],
                    EmailConfirmed = true,
                    RoleId = (int)RoleType.Admin,
                };

                await _userManager.CreateAsync(admin, _configuration["APIUser:Admin:Password"]);
                await _userManager.AddToRoleAsync(admin, Role.ADMIN);
            }
        }
    }
}