using AutoMapper;
using JB.Infrastructure.Constants;
using JB.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace JB.API.User.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = Role.ADMIN)]
    public class ProfileController : ControllerBase
    {
        private readonly IUserProfileService _profileService;
        private readonly IMapper _mapper;
        public ProfileController(
            IUserProfileService profileService,
            IMapper mapper
            )
        {
            _profileService = profileService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Reindex()
        {
            var unlockUserStatus = await _profileService.Reindex();
            if (!unlockUserStatus.IsSuccess)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
