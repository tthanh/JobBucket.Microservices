using AutoMapper;
using JB.Job.Models.Job;
using JB.Job.Services;
using JB.Job.DTOs.Job;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models.Authentication;
using JB.Infrastructure.DTOs;
using JB.Infrastructure.Helpers;

namespace JB.Job.Controllers
{
    /// <summary>
    /// Job service
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class JobController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserManagementService _userManagementService;
        private readonly IOrganizationService _organizationService;
        private readonly IJobService _jobService;
        private readonly IMapper _mapper;
        private readonly IUserClaimsModel _claims;
        private object listFilter;

        public JobController(
            IConfiguration configuration,
            IMapper mapper,
            IUserClaimsModel claims,
            IUserManagementService userManagementService,
            IOrganizationService organizationService,
            IJobService jobService
            )
        {
            _configuration = configuration;
            _mapper = mapper;
            _claims = claims;
            _userManagementService = userManagementService;
            _organizationService = organizationService;
            _jobService = jobService;
        }

        /// <summary>
        /// Get list of jobs.
        /// </summary>
        /// <param name="listVM"></param>
        /// <returns></returns>
        [HttpGet("listJob")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> ListJobs([FromQuery] ListRequest listVM)
        {

            Expression<Func<JobModel, bool>> filters = ExpressionHelper.True<JobModel>();
            Expression<Func<JobModel, object>> sorts = u => u.Id;
            int size = listVM.Size ?? 10;
            int page = listVM.Page ?? 1;
            bool isDescending = listVM.IsDescending ?? false;
            string orderBy = listVM.SortBy;

            (var status, var jobs) = await _jobService.List(filters, sorts, size, page);
            if (!status.IsSuccess)
            {
                return StatusCode(404, status.Message);
            }

            var result = jobs.Select(j => _mapper.Map<JobResponse>(j));

            return Ok(result);
           
        }

        /// <summary>
        /// Get job details by job id
        /// </summary>
        /// <param name="jobId">User id</param>
        /// <returns></returns>
        [HttpGet("{jobId}/details")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetDetails([FromRoute][Required] int jobId)
        {
            (var status, var job) = await _jobService.GetById(jobId);
            if (!status.IsSuccess)
            {
                return StatusCode(404, status.Message);
            }

            return Ok(_mapper.Map<JobResponse>(job));
        }


        /// <summary>
        /// add job
        /// </summary>
        /// <param name="jobVM">Job Request VM</param>
        /// <returns></returns>
        [HttpPost("")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles =Role.RECRUITER + "," + Role.ADMIN)]
        public async Task<IActionResult> AddJob([FromBody] UpdateJobRequest jobVM)
        {
            
            JobModel jobModel = _mapper.Map<JobModel>(jobVM);

            var status = await _jobService.Add(jobModel);
            if (!status.IsSuccess)
            {
                return StatusCode(404, status.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Count job. Use for Admin only
        /// </summary>
        /// <returns></returns>
        [HttpGet("count")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = Role.CUSTOMER_CARE + "," + Role.ADMIN)]
        public async Task<IActionResult> CountJob()
        {
            Expression<Func<JobModel, bool>> filters = ExpressionHelper.True<JobModel>();

            (var status, var jobCount) = await _jobService.Count(filters);
            if (!status.IsSuccess)
            {
                return StatusCode(404, status.Message);
            }

            return Ok(new
            {
                jobCount,
            });
        }

        /// <summary>
        /// delete job by job id
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpDelete("{jobId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = Role.RECRUITER + "," + Role.ADMIN)]
        public async Task<IActionResult> DeleteJob([FromRoute] int jobId)
        {
           var status = await _jobService.Delete(jobId);
            if (!status.IsSuccess)
            {
                return StatusCode(404, status.Message);
            }

            return Ok();
        }
        /// <summary>
        /// update job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobVM"></param>
        /// <returns></returns>
        [HttpPut("{jobId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = Role.RECRUITER + "," + Role.ADMIN)]
        public async Task<IActionResult> UpdateJob(int jobId, [FromBody] UpdateJobRequest jobVM)
        {
            JobModel jobModel = _jobService.GetById(jobId).Result.Item2;
            _mapper.Map(jobVM, jobModel);
            var status = await _jobService.Update(jobModel);

            if (!status.IsSuccess)
            {
                return StatusCode(404, status.Message);
            }
            return Ok();
        }
    }
}
