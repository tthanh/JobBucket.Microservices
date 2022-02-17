using System;
using System.Net.Http.Headers;
using JB.Gateway.DTOs.Chat;
using JB.Gateway.DTOs.Notification;
using JB.Gateway.GraphQL.Subscriptions;
using JB.Gateway.MessageBus.Consumers;
using JB.Gateway.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SlimMessageBus;
using SlimMessageBus.Host.MsDependencyInjection;
using SlimMessageBus.Host.Redis;
using SlimMessageBus.Host.Serialization.Json;
using StackExchange.Redis;

namespace JB.Gateway
{
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
            services.AddHttpContextAccessor();
            services.AddSingleton<NotificationSubscriptions>();

            var graphQLBuilder = services.AddGraphQLServer()
                .AddInMemorySubscriptions();
                //.AddRedisSubscriptions((sp) => ConnectionMultiplexer.Connect(Configuration["Redis:Url"]));

            var graphqQLDownstreams = Configuration.GetSection("GraphQL:Downstreams").Get<GraphQLDownstreamInformation[]>();
            if (graphqQLDownstreams?.Length > 0)
            {
                foreach (var downstream in graphqQLDownstreams)
                {
                    if (!string.IsNullOrEmpty(downstream.Name) && !string.IsNullOrEmpty(downstream.Url))
                    {
                        services.AddHttpClient(downstream.Name, (sp, c) =>
                        {
                            HttpContext context = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;

                            if (context.Request.Headers.ContainsKey("Authorization"))
                            {
                                c.DefaultRequestHeaders.Authorization =
                                    AuthenticationHeaderValue.Parse(
                                        context.Request.Headers["Authorization"]
                                            .ToString());
                            }

                            c.BaseAddress = new Uri(downstream.Url);
                        });
                        graphQLBuilder = graphQLBuilder.AddRemoteSchema(downstream.Name);
                    }
                }
            }

            graphQLBuilder = graphQLBuilder
                .AddTypeExtension<NotificationSubscriptions>()
                .AddTypeExtension<ChatSubscriptions>();

            services.AddCors(o => o.AddPolicy("LowCorsPolicy", builder =>
            {
                builder.SetIsOriginAllowed(origin => true)
                       .AllowCredentials()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddTransient<IJwtService, JwtService>();
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

            #region PubSub
            //var notiConsumer = new NotificationGraphQLConsumer();
            //var chatConsumer = new ChatGraphQLConsumer();

            services.AddSingleton<NotificationGraphQLConsumer>(/*notiConsumer*/);
            services.AddSingleton<ChatGraphQLConsumer>(/*chatConsumer*/);

            services.AddSlimMessageBus((mbb, svp) =>
            {
                mbb
                    .Consume<SubscriptionsNotificationResponse>(x =>
                    {
                        x.Topic("graphql_notification").WithConsumer<NotificationGraphQLConsumer>();
                    })
                    .Consume<SubscriptionsMessageResponse>(x =>
                    {
                        x.Topic("graphql_chat").WithConsumer<ChatGraphQLConsumer>();
                    })
                    .WithProviderRedis(new RedisMessageBusSettings(Configuration["Redis:Url"]))
                    .WithSerializer(new JsonMessageSerializer());
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("LowCorsPolicy");
            app.UseRouting();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });

            app.ApplicationServices.GetRequiredService<IMessageBus>();
           
            app.UseOcelot().Wait();
        }
    }
}
