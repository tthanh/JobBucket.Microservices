using HotChocolate;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using JB.Notification.Models.Chat;
using JB.Notification.Models.Notification;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.GraphQL.Chat
{
    public static class GraphQLChatExtensions
    {
        public static IServiceCollection AddGraphQLChat(this IServiceCollection services)
        {
            services.AddScoped<ChatQuery>();
            services.AddScoped<ChatMutation>();
            //services.AddSingleton<IObserver<ChatMessageModel>, ChatSubscriptions>();

            services.AddScoped<ChatMutationWrapper>();

            services.AddGraphQLServer()
                .AddTypeExtension<ChatQuery>()
                .AddTypeExtension<ChatMutationWrapper>();
                //.AddTypeExtension<ChatSubscriptions>();

            return services;
        }
    }

    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class ChatMutationWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        public ChatMutationWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [GraphQLName("chat")]
        public ChatMutation Chat() => _serviceProvider.GetRequiredService<ChatMutation>();
    }
}
