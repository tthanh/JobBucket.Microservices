using HotChocolate;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using JB.Organization.Models.Notification;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.GraphQL.Review
{
    public static class GraphQLReviewExtensions
    {
        public static IServiceCollection AddGraphQLReview(this IServiceCollection services)
        {
            services.AddScoped<ReviewQuery>();
            services.AddScoped<ReviewMutation>();

            services.AddScoped<ReviewMutationWrapper>();

            services.AddGraphQLServer()
                .AddTypeExtension<ReviewQuery>()
                .AddTypeExtension<ReviewMutationWrapper>();

            return services;
        }
    }

    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class ReviewMutationWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        public ReviewMutationWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [GraphQLName("review")]
        public ReviewMutation Review() => _serviceProvider.GetRequiredService<ReviewMutation>();
    }
}
