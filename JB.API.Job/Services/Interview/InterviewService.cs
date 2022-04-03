using AutoMapper;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Job.Constants;
using JB.Job.Data;
using JB.Job.Helpers;
using JB.Job.Models.Interview;
using JB.Job.Models.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JB.Job.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly InterviewDbContext _interviewDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<InterviewService> _logger;
        private readonly IUserClaimsModel _claims;

        private readonly IUserManagementService _userService;
        private readonly IOrganizationService _organizationService;
        private readonly ICVService _cvService;
        private readonly IJobService _jobService;
        private readonly INotificationService _notiService;

        public InterviewService(
            InterviewDbContext interviewDbContext,
            IMapper mapper,
            ILogger<InterviewService> logger,
            IUserClaimsModel claims,
            IUserManagementService userService,
            IOrganizationService organizationService,
            ICVService cvService,
            IJobService jobService,
            INotificationService notiService
        )
        {
            _interviewDbContext = interviewDbContext;
            _mapper = mapper;
            _logger = logger;
            _claims = claims;
            _userService = userService;
            _organizationService = organizationService;
            _cvService = cvService;
            _jobService = jobService;
            _notiService = notiService;
        }

        public async Task<(Status, InterviewModel)> PassInterview(int interviewId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            InterviewModel interview = null;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (interviewId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
               

                try
                {
                    interview = await _interviewDbContext.Interviews.FirstOrDefaultAsync(x => x.Id == interviewId);
                    if (interview == null)
                    {
                        result.ErrorCode = ErrorCode.InterviewNull;
                        break;
                    }
                    (var status, var organization) = await _organizationService.GetById(interview.OrganizationId);
                    if (status.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }
                    if (!UserHelper.IsManager(userId, organization) &&
                        userId != interview.InterviewerId)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }


                    interview.Status = (int)InterviewStatus.Passed;
                    _interviewDbContext.Interviews.Update(interview);
                    await _interviewDbContext.SaveChangesAsync();
                    await _jobService.PassApplication(interview.JobId, interview.IntervieweeId);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, interview);
        }

        public async Task<(Status, InterviewModel)> FailInterview(int interviewId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            InterviewModel interview = null;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (interviewId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }


                try
                {
                    interview = await _interviewDbContext.Interviews.FirstOrDefaultAsync(x => x.Id == interviewId);
                    if (interview == null)
                    {
                        result.ErrorCode = ErrorCode.InterviewNull;
                        break;
                    }
                    (var status, var organization) = await _organizationService.GetById(interview.OrganizationId);
                    if (status.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }
                    if (!UserHelper.IsManager(userId, organization) &&
                        userId != interview.InterviewerId)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }


                    interview.Status = (int)InterviewStatus.Failed;
                    _interviewDbContext.Interviews.Update(interview);
                    await _interviewDbContext.SaveChangesAsync();
                    await _jobService.FailApplication(interview.JobId, interview.IntervieweeId);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, interview);
        }

        public async Task<(Status, InterviewModel)> NextInterview(int interviewId, DateTime newDate)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            InterviewModel interview = null;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (interviewId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }


                try
                {
                    interview = await _interviewDbContext.Interviews.FirstOrDefaultAsync(x => x.Id == interviewId);
                    if (interview == null)
                    {
                        result.ErrorCode = ErrorCode.InterviewNull;
                        break;
                    }

                    (var status, var organization) = await _organizationService.GetById(interview.OrganizationId);
                    if (status.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }
                    if (!UserHelper.IsManager(userId, organization) &&
                        userId != interview.InterviewerId)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }


                    interview.Status = (int)InterviewStatus.Unverified;
                    interview.InterviewTime = newDate;
                    interview.CurrentInterviewRound += 1;
                    _interviewDbContext.Interviews.Update(interview);
                    await _interviewDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, interview);
        }

        public async Task<Status> AcceptInterview(int interviewId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            InterviewModel interview = null;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (interviewId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }


                try
                {
                    interview = await _interviewDbContext.Interviews.FirstOrDefaultAsync(x => x.Id == interviewId);
                    if (interview == null)
                    {
                        result.ErrorCode = ErrorCode.InterviewNull;
                        break;
                    }
                    if(interview.IntervieweeId != userId)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    interview.Status = (int)InterviewStatus.Accepted;
                    _interviewDbContext.Interviews.Update(interview);
                    await _interviewDbContext.SaveChangesAsync();

                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<Status> DenyInterview(int interviewId)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            InterviewModel interview = null;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (interviewId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }


                try
                {
                    interview = await _interviewDbContext.Interviews.FirstOrDefaultAsync(x => x.Id == interviewId);
                    if (interview == null)
                    {
                        result.ErrorCode = ErrorCode.InterviewNull;
                        break;
                    }
                    if (interview.IntervieweeId != userId)
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    interview.Status = (int)InterviewStatus.Denied;
                    _interviewDbContext.Interviews.Update(interview);
                    await _interviewDbContext.SaveChangesAsync();

                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, InterviewModel)> RescheduleInterview(int interviewId, DateTime newInterviewTime)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            InterviewModel interview = null;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (interviewId <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
                

                try
                {
                    interview = await _interviewDbContext.Interviews.FirstOrDefaultAsync(x => x.Id == interviewId);
                    if (interview == null)
                    {
                        result.ErrorCode = ErrorCode.InterviewNull;
                        break;
                    }
                    (var status, var organization) = await _organizationService.GetById(interview.OrganizationId);
                    if (status.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }

                    if (!UserHelper.IsManager(userId, organization) &&
                        userId != interview.InterviewerId) {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    //checking
                  

                    interview.Status = (int)InterviewStatus.Unverified;
                    interview.InterviewTime = newInterviewTime;
                    _interviewDbContext.Interviews.Update(interview);
                    await _interviewDbContext.SaveChangesAsync();

                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, interview);
        }


        public async Task<Status> Add(InterviewModel entity)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            do
            {
                if (entity == null ||
                    entity.IntervieweeId <= 0 ||
                    entity.InterviewerId <= 0 ||
                    entity.JobId <= 0
                    //entity.IntervieweeCVId <= 0
                    )
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                int organizationId = _userService.GetUser(userId).Result.Item2?.OrganizationId ?? 0;
                if (organizationId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserIsNotRecruiter;
                    break;
                }

                entity.OrganizationId = organizationId;
                entity.Status = (int)InterviewStatus.Unverified;
                entity.CurrentInterviewRound = 1;
                try
                {

                    await _interviewDbContext.Interviews.AddAsync(entity);
                    await _interviewDbContext.SaveChangesAsync();

                    _interviewDbContext.Entry(entity).State = EntityState.Detached;
                    entity = await _interviewDbContext.Interviews.FindAsync(entity.Id);

                    await _jobService.ProcessingApplication(entity.JobId, entity.IntervieweeId);
                    await _notiService.Add(new NotificationModel
                    {
                        Message = $"Interviewed scheduled waiting for confirmation",
                        SenderId = userId,
                        ReceiverId = entity.IntervieweeId,
                    });
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

        public async Task<(Status, long)> Count(Expression<Func<InterviewModel, bool>> predicate)
        {
            Status result = new Status();
            long count = 0;
            do
            {
                try
                {
                    if (predicate == null)
                    {
                        result.ErrorCode = ErrorCode.InvalidArgument;
                        break;
                    }

                    count = await _interviewDbContext.Interviews.Where(predicate).CountAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return (result, count);
        }

        public async Task<Status> Delete(int id)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (id <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }


                try
                {
                    var interview = await _interviewDbContext.Interviews.FirstOrDefaultAsync(x => x.Id == id);
                    if (interview == null)
                    {
                        result.ErrorCode = ErrorCode.InterviewNull;
                        break;
                    }

                    (var status, var organization) = await _organizationService.GetById(interview.OrganizationId);
                    if (status.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }

                    if (!UserHelper.IsRecruiter(userId, organization) &&
                        !UserHelper.IsManager(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    _interviewDbContext.Interviews.Remove(interview);
                    await _interviewDbContext.SaveChangesAsync();

                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }



        public async Task<(Status, InterviewModel)> GetById(int id)
        {
            Status result = new Status();
            InterviewModel interview = null;
            int userId = _claims?.Id ?? 0;
            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                if (id <= 0)
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }
                try
                {
                    interview = await _interviewDbContext.Interviews.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (interview == null)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }

                    (var status, var organization) = await _organizationService.GetById(interview.OrganizationId);
                    if (status.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.OrganizationNull;
                        break;
                    }

                    if (userId != interview.IntervieweeId &&
                        !UserHelper.IsRecruiter(userId, organization) && !UserHelper.IsManager(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }
                    interview.Organization = organization;

                    (var statusEmp, var emp) = await _userService.GetUser(interview.IntervieweeId);
                    if (statusEmp.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }
                    interview.Interviewee = emp;

                    (var statusEmper, var emper) = await _userService.GetUser(interview.InterviewerId);
                    if (statusEmper.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }
                    interview.Interviewer = emper;

                    (var statusjob, var job) = await _jobService.GetById(interview.JobId);
                    if (statusjob.ErrorCode != ErrorCode.Success)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }
                    interview.Job = job;

                    (var statusCV, var cv) = await _cvService.GetById(interview.IntervieweeCVId);
                    //if(statusCV.ErrorCode != ErrorCode.Success)
                    //{
                    //    result.ErrorCode = ErrorCode.cvNull;
                    //    break;
                    //}
                    interview.IntervieweeCV = cv;
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, interview);
        }

        public async Task<(Status, List<InterviewModel>)> List(Expression<Func<InterviewModel, bool>> filter, Expression<Func<InterviewModel, object>> sort, int size, int offset, bool isDescending = false)
        {
            Status result = new Status();
            var interviews = new List<InterviewModel>();
            int userId = _claims?.Id ?? 0;

            do
            {
                if (userId <= 0)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }
                (var status, var user) = await _userService.GetUser(userId);
                if (status.ErrorCode != ErrorCode.Success)
                {
                    result.ErrorCode = ErrorCode.UserNotExist;
                    break;
                }



                try
                {
                    IQueryable<InterviewModel> preQuery = null;
                    if (user.RoleId == (int)RoleType.Recruiter || user.RoleId == (int)RoleType.OrganizationManager)
                    {
                        if (user.OrganizationId <= 0)
                        {
                            result.ErrorCode = ErrorCode.OrganizationNull;
                            break;
                        }
                        preQuery = _interviewDbContext.Interviews.Where(i => i.OrganizationId == user.OrganizationId);
                    }
                    else
                    {
                        preQuery = _interviewDbContext.Interviews.Where(i => i.IntervieweeId == userId);
                    }

                    var interviewQuery = preQuery.Where(filter);
                    interviewQuery = isDescending ? interviewQuery.OrderByDescending(sort) : interviewQuery.OrderBy(sort);
                    interviews = await interviewQuery.Skip(size * (offset - 1)).Take(size).ToListAsync();
                    if (interviews == null)
                    {
                        result.ErrorCode = ErrorCode.InterviewNull;
                        break;
                    }

                    foreach (var interview in interviews)
                    {
                        (var statusOrg, var organization) = await _organizationService.GetById(interview.OrganizationId);
                        if (statusOrg.ErrorCode != ErrorCode.Success)
                        {
                            result.ErrorCode = ErrorCode.OrganizationNull;
                            break;
                        }

                        if (userId != interview.IntervieweeId &&
                            !UserHelper.IsRecruiter(userId, organization) && !UserHelper.IsManager(userId, organization))
                        {
                            result.ErrorCode = ErrorCode.NoPrivilege;
                            break;
                        }
                        interview.Organization = organization;

                        (var statusEmp, var emp) = await _userService.GetUser(interview.IntervieweeId);
                        if (statusEmp.ErrorCode != ErrorCode.Success)
                        {
                            result.ErrorCode = ErrorCode.UserNotExist;
                            break;
                        }
                        interview.Interviewee = emp;

                        (var statusEmper, var emper) = await _userService.GetUser(interview.InterviewerId);
                        if (statusEmper.ErrorCode != ErrorCode.Success)
                        {
                            result.ErrorCode = ErrorCode.UserNotExist;
                            break;
                        }
                        interview.Interviewer = emper;

                        (var statusjob, var job) = await _jobService.GetById(interview.JobId);
                        if (statusjob.ErrorCode != ErrorCode.Success)
                        {
                            result.ErrorCode = ErrorCode.JobNull;
                            break;
                        }
                        interview.Job = job;

                        (var statusCV, var cv) = await _cvService.GetById(interview.IntervieweeCVId);
                        //if (statusCV.ErrorCode != ErrorCode.Success)
                        //{
                        //    result.ErrorCode = ErrorCode.cvNull;
                        //    break;
                        //}
                        interview.IntervieweeCV = cv;
                    }

                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }

            }
            while (false);

            return (result, interviews);
        }


        public async Task<Status> Update(InterviewModel entity)
        {
            Status result = new Status();
            int userId = _claims?.Id ?? 0;

            do
            {
                try
                {
                    if (entity == null)
                    {
                        result.ErrorCode = ErrorCode.JobNull;
                        break;
                    }
                    if (userId <= 0)
                    {
                        result.ErrorCode = ErrorCode.UserNotExist;
                        break;
                    }


                    (var status, var organization) = await _organizationService.GetById(entity.OrganizationId);
                    if (!UserHelper.IsRecruiter(userId, organization) && !UserHelper.IsManager(userId, organization))
                    {
                        result.ErrorCode = ErrorCode.NoPrivilege;
                        break;
                    }

                    var interview = await _interviewDbContext.Interviews.Where(x => x.Id == entity.Id).FirstOrDefaultAsync();
                    PropertyHelper.InjectNonNull<InterviewModel>(interview, entity);
                    _interviewDbContext.Update(interview);
                    await _interviewDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.Unknown;
                    _logger.LogError(e, e.Message);
                }
            }
            while (false);

            return result;
        }

      
    }
}
