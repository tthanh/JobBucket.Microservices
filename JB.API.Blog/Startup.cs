using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using JB.Blog.Services;
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
using JB.Blog.AutoMapper;
using JB.Blog.Data;
using JB.Blog.GraphQL.Blog;
using JB.API.Infrastructure.Middlewares;
using Newtonsoft.Json;
using SlimMessageBus.Host.Redis;
using SlimMessageBus.Host.Serialization.Json;
using JB.Infrastructure.Messages;
using SlimMessageBus.Host.MsDependencyInjection;

namespace JB.Blog
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

            services.AddDbContext<BlogDbContext>(dbOptions);

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
                typeof(BlogMapperProfile).Assembly
                );

            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IUserManagementService, UserManagementGRPCService>();
            services.AddScoped<INotificationService, NotificationGRPCService>();
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
                .AddInMemorySubscriptions()
                .AddQueryType()
                .AddMutationType();

            services.AddGraphQLBlog();
            #endregion

            #region gRPC services
            services.AddGrpcClient<JB.gRPC.User.UserRPC.UserRPCClient>(c =>
            {
                c.Address = new Uri(Configuration["GrpcServices:User"]);
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
