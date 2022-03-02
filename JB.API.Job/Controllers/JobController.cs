using AutoMapper;
using JB.Infrastructure.Constants;
using JB.Job.DTOs.Job;
using JB.Job.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JB.API.Job.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = Role.ADMIN)]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IMapper _mapper;
        public JobController(
            IJobService jobService,
            IMapper mapper
            )
        {
            _jobService = jobService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page, [FromQuery] int size)
        {
            (var getStatus, var jobs) = await _jobService.List(u => true, u => u.Id, size, page);
            if (!getStatus.IsSuccess)
            {
                return NotFound(getStatus.Message);
            }

            var results = _mapper.Map<List<JobResponse>>(jobs);

            return Ok(results);
        }

        [HttpPut("{jobId}")]
        public async Task<IActionResult> Lock([FromRoute][Required] int jobId)
        {
            var lockUserStatus = await _jobService.Lock(jobId);
            if (!lockUserStatus.IsSuccess)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("{jobId}")]
        public async Task<IActionResult> Unlock([FromRoute][Required] int jobId)
        {
            var unlockUserStatus = await _jobService.Unlock(jobId);
            if (!unlockUserStatus.IsSuccess)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Reindex()
        {
            var unlockUserStatus = await _jobService.Unlock(jobId);
            if (!unlockUserStatus.IsSuccess)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
