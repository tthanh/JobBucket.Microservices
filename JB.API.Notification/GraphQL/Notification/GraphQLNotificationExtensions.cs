using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using JB.Notification.Models.Notification;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Notification.GraphQL.Notification
{
    public static class GraphQLNotificationExtensions
    {
        public static IServiceCollection AddGraphQLNotification(this IServiceCollection services)
        {
            services.AddScoped<NotificationQuery>();
            services.AddScoped<NotificationMutation>();
            services.AddSingleton<IObserver<NotificationModel>, NotificationSubscriptions>();

            services.AddScoped<NotificationMutationWrapper>();

            services.AddGraphQLServer()
                .AddTypeExtension<NotificationQuery>()
                .AddTypeExtension<NotificationMutationWrapper>()
                .AddTypeExtension<NotificationSubscriptions>();

            return services;
        }
    }

    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class NotificationMutationWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        public NotificationMutationWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public NotificationMutation Notification() => _serviceProvider.GetRequiredService<NotificationMutation>();
    }
}
