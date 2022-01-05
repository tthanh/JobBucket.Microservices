using HotChocolate;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JB.User.GraphQL.CV
{
    public static class GraphQLCVExtensions
    {
        public static IServiceCollection AddGraphQLCV(this IServiceCollection services)
        {
            services.AddScoped<CVQuery>();
            services.AddScoped<CVMutation>();

            services.AddScoped<CVMutationWrapper>();

            services.AddGraphQLServer()
                .AddTypeExtension<CVQuery>()
                .AddTypeExtension<CVMutationWrapper>();

            return services;
        }
    }

    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class CVMutationWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        public CVMutationWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [GraphQLName("cv")]
        public CVMutation CV() => _serviceProvider.GetRequiredService<CVMutation>();
    }
}
