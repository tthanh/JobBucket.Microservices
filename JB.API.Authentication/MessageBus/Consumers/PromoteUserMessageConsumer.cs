using AutoMapper;
using JB.Authentication.Services;
using JB.Infrastructure.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SlimMessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Authentication.MessageBus.Consumers
{
    public class PromoteUserMessageConsumer : IConsumer<PromoteUserMessage>
    {
        private IMapper _mapper;
        private IServiceProvider _serviceProvider;
        
        public PromoteUserMessageConsumer(IMapper mapper,
            IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }
        public async Task OnHandle(PromoteUserMessage message, string path)
        {
            using var scope = _serviceProvider.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserManagementService>();
            _ = await userService.PromoteTo(message.UserId, message.OrganizationId, message.RoleId);
        }
    }
}
   