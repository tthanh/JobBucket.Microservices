using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using JB.Notification.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Threading.Tasks;
using Npgsql;
using Nest;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Models.Authentication;
using JB.Notification.AutoMapper;
using JB.Notification.GraphQL.Notification;
using JB.Notification.GraphQL.Chat;
using JB.Notification.Data;
using JB.API.Infrastructure.Middlewares;
using JB.Notification.Models.Notification;
using JB.API.Notification.GraphQL.Sample;
using JB.Infrastructure.Messages;
using SlimMessageBus.Host.Redis;
using SlimMessageBus.Host.Serialization.Json;
using Newtonsoft.Json;
using JB.API.Notification.MessageBus.Consumers;
using SlimMessageBus.Host.MsDependencyInjection;
using StackExchange.Redis;
using JB.Notification.Models.Chat;
using Newtonsoft.Json.Linq;
using SlimMessageBus;
using JB.Infrastructure.DTOs.Subscriptions;
using Google.Apis.Auth.OAuth2;
using ErrorCode = JB.Infrastructure.Constants.ErrorCode;

namespace JB.Notification
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Config
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddLogging();
            #endregion

            #region Database configuration

            var dbOptions = new Action<DbContextOptionsBuilder>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("Database"),
                        optionsBuilder =>
                        optionsBuilder.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name));
            });

            services.AddDbContext<ChatDbContext>(dbOptions);
            services.AddDbContext<NotificationDbContext>(dbOptions);

            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            #endregion

            #region Elasticsearch
            var connectionSettings = new ConnectionSettings(new Uri(Configuration["Elasticsearch:Url"]));
            var client = new ElasticClient(connectionSettings);
            services.AddSingleton<IElasticClient>(client);
            #endregion

            #region Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["Redis:Url"];
                options.InstanceName = "JB.API";
            });
            #endregion

            #region Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddAuthorization();

            services.AddScoped<IUserClaimsModel, UserClaimsModel>();
            #endregion

            #region Services
            services.AddAutoMapper(
                typeof(NotificationMapperProfile).Assembly,
                typeof(ChatMapperProfile).Assembly
                );

            services.AddTransient<IJwtService, JwtService>();
            services.AddScoped<IOrganizationService, OrganizationGRPCService>();
            services.AddScoped<IUserManagementService, UserManagementGRPCService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IChatService, ChatService>();

            services.AddSingleton<NotificationRedisPubSubObserver>();
            services.AddSingleton<IObserver<NotificationModel>, NotificationRedisPubSubObserver>(p => p.GetService<NotificationRedisPubSubObserver>());
            services.AddSingleton<NotificationFirebaseObserver>();
            services.AddSingleton<IObserver<NotificationModel>, NotificationFirebaseObserver>(p => p.GetService<NotificationFirebaseObserver>());

            services.AddSingleton<ChatRedisPubSubObserver>();
            services.AddSingleton<IObserver<ChatMessageModel>, ChatRedisPubSubObserver>(p => p.GetService<ChatRedisPubSubObserver>());
            services.AddSingleton<ChatFirebaseObserver>();
            services.AddSingleton<IObserver<ChatMessageModel>, ChatFirebaseObserver>(p => p.GetService<ChatFirebaseObserver>());

            services.AddSingleton<INotificationSubscriptionsService, NotificationSubscriptionsService>();
            services.AddSingleton<IChatSubscriptionsService, ChatSubscriptionsService>();
            #endregion

            #region REST endpoints
            services.AddCors(o => o.AddPolicy("LowCorsPolicy", builder =>
            {
                builder.SetIsOriginAllowed(origin => true)
                       .AllowCredentials()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            #endregion

            #region GraphQL
            services.AddGraphQLServer()
                //.AddInMemorySubscriptions()
                .AddQueryType()
                .AddMutationType()
                .AddSubscriptionType()
                .AddTypeExtension<SampleSubscriptions>();

            services.AddGraphQLChat();
            services.AddGraphQLNotification();
            #endregion

            #region gRPC services
            services.AddGrpcClient<JB.gRPC.User.UserRPC.UserRPCClient>(c =>
            {
                c.Address = new Uri(Configuration["GrpcServices:User"]);
            });
            services.AddGrpcClient<JB.gRPC.Organization.OrganizationRPC.OrganizationRPCClient>(c =>
            {
                c.Address = new Uri(Configuration["GrpcServices:Organization"]);
            });
            #endregion

            #region PubSub
            services.AddSingleton<NotificationMessageConsumer>();

            services.AddSlimMessageBus((mbb, svp) =>
            {
                mbb
                    .Produce<SubscriptionsMessageResponse>(p => p.DefaultTopic("graphql_chat"))
                    .Produce<SubscriptionsNotificationResponse>(p => p.DefaultTopic("graphql_notification"))
                    .Consume<NotificationMessage>(x =>
                    {
                        x.Topic("notification").WithConsumer<NotificationMessageConsumer>();
                    })
                    .WithProviderRedis(new RedisMessageBusSettings(Configuration["Redis:Url"]))
                    .WithSerializer(new JsonMessageSerializer());
            });
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("LowCorsPolicy");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            _ = app.UseExceptionHandler(a => a.Run(context =>
              {
                  context.Response.StatusCode = 503;
                  _ = context.Response.WriteAsJsonAsync(new
                  {
                      message = EnumHelper.GetDescriptionFromEnumValue(ErrorCode.ServerError),
                  });

                  return Task.CompletedTask;
              }));

            app.UseMiddleware<JwtMiddleware>();
            app.UseWebSockets();

            app.SubScribeToNotification();
            app.SubScribeToChat();
            
            app.ApplicationServices.GetRequiredService<IMessageBus>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
