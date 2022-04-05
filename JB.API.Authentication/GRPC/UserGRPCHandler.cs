using AutoMapper;
using Grpc.Core;
using JB.Authentication.Models.User;
using JB.Authentication.Services;
using JB.gRPC.Organization;
using JB.gRPC.User;
using JB.Infrastructure.Constants;
using Nest;
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

        public override async Task<gRPC.User.User> Update(UpdateUserRequest request, ServerCallContext context)
        {
            gRPC.User.User userResponse = new gRPC.User.User();

            (var status, var user) = await _userManagementService.GetUser(request.Id);
            if (!status.IsSuccess || user == null)
            {
                return userResponse;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                user.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.AvatarUrl))
            {
                user.AvatarUrl = request.AvatarUrl;
            }

            if (request.DefaultCVId > 0)
            {
                user.DefaultCVId = request.DefaultCVId;
            }
            
            user.ProfileStatus = request.ProfileStatus;

            (status, user) = await _userManagementService.UpdateUser(user);

            return userResponse;
        }

        public override async Task<gRPC.User.User> Create(CreateUserRequest request, ServerCallContext context)
        {
            gRPC.User.User userResponse = new gRPC.User.User();

            var user = new UserModel()
            {
                Name = request.Name,
                UserName = request.UserName,
                Email = request.Email,
                RoleId = request.RoleId,
                OrganizationId = request.OrganizationId,
                PasswordPlain = request.PasswordPlain,
            };

            (var status,var userResult) = await _userManagementService.CreateUser(user);

            userResponse = _mapper.Map<UserModel, gRPC.User.User>(userResult);

            return userResponse;
        }

        public override async Task<UserResponse> Delete(UserRequest request, ServerCallContext context)
        {
            gRPC.User.UserResponse userResponse = new gRPC.User.UserResponse();

            foreach (var id in request.Id)
            {
                var status = await _userManagementService.DeleteUser(id);
            }

            return userResponse;
        }
    }
}
