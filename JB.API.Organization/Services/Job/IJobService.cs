using JB.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.Services
{
    public interface IJobService
    {
        Task<(Status, bool)> IsAnyApplication(int employeeId);
    }
}
