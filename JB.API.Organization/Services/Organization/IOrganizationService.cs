using JB.Organization.Models;
using JB.Organization.Models.Job;
using JB.Organization.Models.Organization;
using JB.Organization.Models.User;
using JB.Infrastructure.Models;
using JB.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Organization.Services
{
    public interface IOrganizationService : IServiceBase<OrganizationModel>
    {
        Task<(Status, List<OrganizationModel>)> GetByIds(List<int> organizationIds);

        //Organization Manager or Recruiter Authorized only
        Task<(Status, OrganizationModel)> GetDetailById(int organizationId);

        //Organization Manager Authorized only
        #region Organization Employer Management
        Task<(Status, UserModel)> AddEmployer(string name, string passwordPlain, string email);
        Task<Status> DeleteEmployer(int Id);
        Task<(Status, string)> ResetPassEmployer(int Id);

        Task<(Status, UserModel)> PromoteEmployer(int Id);
        Task<(Status, UserModel)> DemoteEmployer(int Id);
        //Task<(Status, List<JobModel>)> ListEmployerProductives(int employerId, Expression<Func<JobModel, object>> sort, int size, int offset, bool isDescending = false);
        #endregion

        Task<(Status, OrganizationModel)> UpdateRating(int organizationId, float rating, float ratingBenefit, float ratingLearning, float ratingCulture, float ratingWorkspace);
    }
}
