using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JB.Job.GraphQL.Interview
{
    public static class GraphQLInterviewExtensions
    {
        public static IServiceCollection AddGraphQLInterview(this IServiceCollection services)
        {
            services.AddScoped<InterviewQuery>();
            services.AddScoped<InterviewMutation>();

            services.AddScoped<InterviewMutationWrapper>();

            services.AddGraphQLServer()
                .AddTypeExtension<InterviewQuery>()
                .AddTypeExtension<InterviewMutationWrapper>();

            return services;
        }
    }

    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class InterviewMutationWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        public InterviewMutationWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public InterviewMutation Interview() => _serviceProvider.GetRequiredService<InterviewMutation>();
    }
}
