using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JB.Blog.GraphQL.Blog
{
    public static class GraphQLBlogExtensions
    {
        public static IServiceCollection AddGraphQLBlog(this IServiceCollection services)
        {
            services.AddScoped<BlogQuery>();
            services.AddScoped<BlogMutation>();

            services.AddScoped<BlogMutationWrapper>();

            services.AddGraphQLServer()
                .AddTypeExtension<BlogQuery>()
                .AddTypeExtension<BlogMutationWrapper>();

            return services;
        }
    }

    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class BlogMutationWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        public BlogMutationWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public BlogMutation Blog() => _serviceProvider.GetRequiredService<BlogMutation>();
    }
}
