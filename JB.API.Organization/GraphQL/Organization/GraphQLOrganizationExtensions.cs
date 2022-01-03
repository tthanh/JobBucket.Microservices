using HotChocolate;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using JB.Organization.Models.Notification;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Organization.GraphQL.Organization
{
    public static class GraphQLOrganizationExtensions
    {
        public static IServiceCollection AddGraphQLOrganization(this IServiceCollection services)
        {
            services.AddScoped<OrganizationQuery>();
            services.AddScoped<OrganizationMutation>();

            services.AddScoped<OrganizationMutationWrapper>();

            services.AddGraphQLServer()
                .AddTypeExtension<OrganizationQuery>()
                .AddTypeExtension<OrganizationMutationWrapper>();

            return services;
        }
    }

    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class OrganizationMutationWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        public OrganizationMutationWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [GraphQLName("organization")]
        public OrganizationMutation Organization() => _serviceProvider.GetRequiredService<OrganizationMutation>();
    }
}
