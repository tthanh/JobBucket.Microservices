using HotChocolate;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JB.Job.GraphQL.Job
{
    public static class GraphQLJobExtensions
    {
        public static IServiceCollection AddGraphQLJob(this IServiceCollection services)
        {
            services.AddScoped<JobQuery>();
            services.AddScoped<JobMutation>();

            services.AddScoped<JobMutationWrapper>();

            services.AddGraphQLServer()
                .AddTypeExtension<JobQuery>()
                .AddTypeExtension<JobMutationWrapper>();

            return services;
        }
    }

    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class JobMutationWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        public JobMutationWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [GraphQLName("job")]
        public JobMutation Job() => _serviceProvider.GetRequiredService<JobMutation>();
    }
}
