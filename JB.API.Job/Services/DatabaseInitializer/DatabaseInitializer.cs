using System.Threading.Tasks;
using JB.Job.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JB.Job.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IConfiguration _configuration;
        private readonly InterviewDbContext _interviewDbContext;
        private readonly JobDbContext _jobDbContext;

        public DatabaseInitializer(
            IConfiguration configuration,
            InterviewDbContext interviewDbContext,
            JobDbContext jobDbContext)
        {
            _configuration = configuration;
            _interviewDbContext = interviewDbContext;
            _jobDbContext = jobDbContext;
        }

        public async Task Initialize()
        {
            //create database schema if none exists
            await _interviewDbContext.Database.MigrateAsync();
            await _jobDbContext.Database.MigrateAsync();
        }
    }
}