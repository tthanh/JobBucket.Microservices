﻿using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using JB.Job.DTOs.Job;
using JB.Job.Models.Job;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace JB.Job.Services
{
    public interface IJobService : IServiceBase<JobModel>
    {
        Task<(Status, List<JobModel>)> Search(string keyword, Expression<Func<JobModel, bool>> filter = null, Expression<Func<JobModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
        Task<(Status, List<JobModel>)> Search(ListJobRequest filter = null);
        Task<(Status, List<JobModel>)> GetRecommendations(int[] entityIds = null, Expression<Func<JobModel, bool>> filter = null, Expression<Func<JobModel, object>> sort = null, int size = 10, int offset = 1, bool isDescending = false);
        Task<(Status, List<JobModel>)> GetRecommendations(ListJobRecommendationRequest filter = null);
        Task<(Status, List<JobModel>)> ListJobByOrganization(int organizationId, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending = false);

        #region Interest
        Task<(Status, List<JobModel>)> GetInterestedJobsByUser();
        Task<(Status, long)> CountInterestedUsersByJob(int jobId);
        Task<Status> AddInterestedJob(int jobId);
        Task<Status> RemoveInterestedJob(int jobId);
        #endregion

        #region Apply
        Task<(Status, ApplicationModel)> Apply(ApplicationModel applyForm);
        Task<Status> Unapply(int jobId); // uv hủy apply
        Task<Status> FailApplication(int jobId, int userId); // hr đánh rớt uv
        Task<Status> PassApplication(int jobId, int userId); // hr đánh đậu uv
        Task<Status> ProcessingApplication(int jobId, int userId); // hr đã đặt lịch
        Task<(Status, List<ApplicationModel>)> GetAppliedJobsByUser(Expression<Func<ApplicationModel, bool>> filter = null, Expression<Func<ApplicationModel, object>> sort = null, int size = 30, int offset = 1, bool isDescending = false);
        Task<(Status, List<ApplicationModel>)> GetAppliedUsersByJob(int jobId, Expression<Func<ApplicationModel, bool>> filter = null, Expression<Func<ApplicationModel, object>> sort = null, int size = 30, int offset = 1, bool isDescending = false);
        Task<(Status, List<ApplicationModel>)> ListApply(Expression<Func<ApplicationModel, bool>> filter, Expression<Func<ApplicationModel, object>> sort, int size, int offset, bool isDescending = false);
        Task<(Status, bool)> IsAnyApplication(int employeeId);
        
        #endregion

        #region Properties
        Task<(Status, JobPropertiesResponse)> GetJobProperties();
        Task<(Status, JobCountsResponse)> GetJobCountsByCategory(int count);
        Task<(Status, List<ApplicationCountsResponse>)> GetJobApplicationCounts(ApplicationCountsRequest req);
        #endregion

        #region
        Task<Status> UpdateExpiredJobStatus();
        Task<Status> NotifyRecommendedJob(int frequencyDay);
        #endregion

        #region Admin
        Task<Status> Lock(int id);
        Task<Status> Unlock(int id);
        Task<Status> Reindex();
        #endregion
    }
}
