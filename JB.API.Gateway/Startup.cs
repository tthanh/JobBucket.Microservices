using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JB.API.Gateway.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace JB.API.Gateway
{
    public class Settings
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var graphQLBuilder = services.AddGraphQLServer();
            var graphqQLDownstreams = Configuration.GetSection("GraphQL:Downstreams").Get<GraphQLDownstreamInformation[]>();
            if (graphqQLDownstreams?.Length > 0)
            {
                foreach (var downstream in graphqQLDownstreams)
                {
                    if (!string.IsNullOrEmpty(downstream.Name) && !string.IsNullOrEmpty(downstream.Url))
                    {
                        services.AddHttpClient(downstream.Name, c => c.BaseAddress = new Uri(downstream.Url));
                        graphQLBuilder = graphQLBuilder.AddRemoteSchema(downstream.Name);
                    }
                }
            }


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  new string[] { }
                }
                });
            });

            services.AddSwaggerForOcelot(Configuration);
            services.AddOcelot(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });

            app.UseOcelot().Wait();
        }
    }
}
