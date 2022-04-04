using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using JB.Job.Services;
using JB.Job.Data;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using JB.Job.AutoMapper;
using System.Threading.Tasks;
using Npgsql;
using JB.Job.GraphQL.Interview;
using JB.Job.GraphQL.Job;
using Nest;
using JB.Infrastructure.Constants;
using JB.Infrastructure.Helpers;
using JB.Infrastructure.Models.Authentication;
using JB.API.Infrastructure.Middlewares;
using Hangfire;
using Hangfire.MemoryStorage;
using JB.Infrastructure.Filters;
using JB.Infrastructure.Messages;
using SlimMessageBus.Host.Redis;
using SlimMessageBus.Host.Serialization.Json;
using SlimMessageBus.Host.MsDependencyInjection;
using JB.Job.GRPC;

namespace JB.Job
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

            services.AddDbContext<JobDbContext>(dbOptions);
            services.AddDbContext<InterviewDbContext>(dbOptions);

            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            #endregion

            #region Elasticsearch
            var connectionSettings = new ConnectionSettings(new Uri(Configuration["Elasticsearch:Url"]));
            connectionSettings = connectionSettings.DefaultIndex("job");
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
                typeof(JobMapperProfile).Assembly,
                typeof(InterviewMapperProfile).Assembly
                );

            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IJobSearchService, JobElasticsearchService>();
            services.AddScoped<IJobDocumentElasticsearchService, JobDocumentElasticsearchService>();
            services.AddScoped<IInterviewService, InterviewService>();
            services.AddScoped<IOrganizationService, OrganizationGRPCService>();
            services.AddScoped<IUserManagementService, UserManagementGRPCService>();
            services.AddScoped<IUserProfileService, UserProfileGRPCService>();
            services.AddScoped<ICVService, CVGRPCService>();
            services.AddScoped<INotificationService, NotificationRemoteService>();

            services.AddHangfire(c => c.UseMemoryStorage());
            #endregion

            #region REST endpoints
            services.AddCors(o => o.AddPolicy("LowCorsPolicy", builder =>
            {
                builder.SetIsOriginAllowed(origin => true)
                       .AllowCredentials()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddControllers(config =>
            {
                config.Filters.Add(new ModelValidationActionFilter());
            })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JB.API.Job", Version = "v1" });
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
            services.AddSwaggerGenNewtonsoftSupport();
            #endregion

            #region GraphQL
            var graphQLBuilder = services.AddGraphQLServer()
                .AddQueryType()
                .AddMutationType();

            services.AddGraphQLInterview();
            services.AddGraphQLJob();
            #endregion

            #region gRPC services
            services.AddGrpc();
            
            services.AddGrpcClient<JB.gRPC.User.UserRPC.UserRPCClient>(c =>
            {
                c.Address = new Uri(Configuration["GrpcServices:User"]);
            });
            services.AddGrpcClient<JB.gRPC.Organization.OrganizationRPC.OrganizationRPCClient>(c =>
            {
                c.Address = new Uri(Configuration["GrpcServices:Organization"]);
            });
            services.AddGrpcClient<JB.gRPC.Profile.ProfileRPC.ProfileRPCClient>(c =>
            {
                c.Address = new Uri(Configuration["GrpcServices:Profile"]);
            });
            
            services.AddGrpcClient<JB.gRPC.CV.CVRPC.CVRPCClient>(c =>
            {
                c.Address = new Uri(Configuration["GrpcServices:CV"]);
            });
            #endregion

            #region PubSub
            services.AddSlimMessageBus((mbb, svp) =>
            {
                mbb
                    .Produce<NotificationMessage>(x =>
                    {
                        x.DefaultTopic("notification");
                    })
                    .WithProviderRedis(new RedisMessageBusSettings(Configuration["Redis:Url"]))
                    .WithSerializer(new JsonMessageSerializer());

            });
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager jobManager)
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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Job");
            });

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            jobManager.AddOrUpdate<IJobService>("update-expired-job", x => x.UpdateExpiredJobStatus(), "0 1 * * *", TimeZoneInfo.Utc);
            jobManager.AddOrUpdate<IJobService>("update-expired-job", x => x.UpdateExpiredJobStatus(), "0 3 * * *", TimeZoneInfo.Utc);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers();
                endpoints.MapGrpcService<JobGRPCHandler>();
            });
        }
    }
}
