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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JB.API.Gateway", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JB.API.Gateway v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
        }
    }
}
