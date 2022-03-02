using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using JB.Authentication.DTOs.Authentication;
using JB.Authentication.Services;
using JB.Authentication.Models.User;
using Swashbuckle.AspNetCore.Annotations;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using JB.Infrastructure.Models.Authentication;
using JB.Authentication.Helpers;
using Microsoft.AspNetCore.Authorization;
using JB.Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq.Expressions;
using JB.Authentication.DTOs.UserManagement;

namespace JB.Authentication.User.Controllers
{
    /// <summary>
    /// Authenticate service
    /// </summary>
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = Role.ADMIN)]

    public class UserManagementController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserManagementService _userManagementService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IUserClaimsModel _claims;

        public UserManagementController(
            IConfiguration configuration,
            IJwtService jwtService,
            IMapper mapper,
            IUserClaimsModel claims,
            IUserManagementService userManagementService
            )
        {
            _configuration = configuration;
            _jwtService = jwtService;
            _mapper = mapper;
            _claims = claims;
            _userManagementService = userManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page, [FromQuery] int size)
        {
            (var getUserStatus, var users) = await _userManagementService.ListUser(u => u.RoleId != (int)RoleType.Admin, u => u.Id, size, page);
            if (!getUserStatus.IsSuccess)
            {
                return NotFound(getUserStatus.Message);
            }

            var results = _mapper.Map<List<GetUserResponse>>(users);

            return Ok(results);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Lock([FromRoute][Required] int userId)
        {
            var lockUserStatus = await _userManagementService.LockUser(userId, DateTime.UtcNow.AddDays(int.MaxValue));
            if (!lockUserStatus.IsSuccess)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Unlock([FromRoute][Required] int userId)
        {
            var unlockUserStatus = await _userManagementService.UnlockUser(userId);
            if (!unlockUserStatus.IsSuccess)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}