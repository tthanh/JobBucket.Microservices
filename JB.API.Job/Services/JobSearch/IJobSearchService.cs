using AutoMapper;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.Services;
using JB.Job.Data;
using JB.Job.DTOs.Job;
using JB.Job.Models.Job;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Status = JB.Infrastructure.Models.Status;

namespace JB.Job.Services
{
    public interface IJobSearchService
    {
        Task<(Status, List<JobModel>)> Search(string keyword, Expression<Func<JobModel, bool>> filter = null, Expression<Func<JobModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
        Task<(Status, List<JobModel>)> Search(int[] entityIds = null, Expression<Func<JobModel, bool>> filter = null, Expression<Func<JobModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
        Task<(Status, List<JobModel>)> Search(ListJobRequest filter = null);
        Task<(Status, List<JobModel>)> Search(ListJobRecommendationRequest filter = null);
    }
}
