using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JB.User.GraphQL.Profile
{
    public static class GraphQLProfileExtensions
    {
        public static IServiceCollection AddGraphQLProfile(this IServiceCollection services)
        {
            services.AddScoped<ProfileQuery>();
            services.AddScoped<ProfileMutation>();

            services.AddScoped<ProfileMutationWrapper>();

            services.AddGraphQLServer()
                .AddTypeExtension<ProfileQuery>()
                .AddTypeExtension<ProfileMutationWrapper>();

            return services;
        }
    }

    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class ProfileMutationWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        public ProfileMutationWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ProfileMutation Profile() => _serviceProvider.GetRequiredService<ProfileMutation>();
    }
}
