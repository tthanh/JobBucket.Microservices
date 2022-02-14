using AutoMapper;
using Grpc.Core;
using JB.Authentication.Models.User;
using JB.Authentication.Services;
using JB.gRPC.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Authentication.GRPC
{
    public class UserGRPCHandler : UserRPC.UserRPCBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IMapper _mapper;
        public UserGRPCHandler(
            IUserManagementService userManagementService,
            IMapper mapper
            )
        {
            _userManagementService = userManagementService;
            _mapper = mapper;
        }

        public override async Task<UserResponse> Get(UserRequest request, ServerCallContext context)
        {
            UserResponse userResponse = new UserResponse();

            (var status, var users) = await _userManagementService.GetUsers(request.Id.ToList());
            if (status.IsSuccess)
            {
                userResponse.Users.AddRange(_mapper.Map<List<UserModel>, List<gRPC.User.User>>(users));
            }

            return userResponse;
        }
    }
}
