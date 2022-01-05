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
                options.InstanceName = "JB.Notification.Redis";
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

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IChatService, ChatService>();
            #endregion

            #region GraphQL
            services.AddGraphQLServer()
                .AddInMemorySubscriptions()
                .AddQueryType()
                .AddMutationType()
                .AddSubscriptionType();

            services.AddGraphQLChat();
            services.AddGraphQLNotification();
            #endregion

            #region gRPC services
            //services.AddGrpcClient<Hello.HelloClient>(c =>
            //{
            //    c.Address = new Uri("http://localhost:50051");
            //});
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

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
