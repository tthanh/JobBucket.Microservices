using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using JB.User.Services;
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
using JB.User.GraphQL.CV;
using JB.User.GraphQL.Profile;
using JB.User.AutoMapper;
using JB.User.Data;
using JB.API.Infrastructure.Middlewares;
using JB.User.Models.Profile;
using JB.Infrastructure.Services;
using JB.User.GRPC;
using JB.Infrastructure.Filters;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;

namespace JB.User
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

            services.AddDbContext<CVDbContext>(dbOptions);
            services.AddDbContext<ProfileDbContext>(dbOptions);

            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            #endregion

            #region Elasticsearch
            var connectionSettings = new ConnectionSettings(new Uri(Configuration["Elasticsearch:Url"])).DefaultIndex("profile");
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
                typeof(CVMapperProfile).Assembly,
                typeof(ProfileMapperProfile).Assembly
                );

            services.AddScoped<IUserManagementService, UserManagementGRPCService>();
            services.AddScoped<IOrganizationService, OrganizationGRPCService>();
            services.AddScoped<IJobService, JobGRPCService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IUserProfileSearchService, UserProfileElasticsearchService>();
            services.AddScoped<IUserProfileDocumentElasticsearchService, UserProfileDocumentElasticsearchService>();
            services.AddScoped<ICVService, CVService>();
            #endregion

            #region GraphQL
            services.AddGraphQLServer()
                .AddInMemorySubscriptions()
                .AddQueryType()
                .AddMutationType();

            services.AddGraphQLCV();
            services.AddGraphQLProfile();
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
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JB.API.Authentication", Version = "v1" });
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
            services.AddGrpcClient<JB.gRPC.Job.JobRPC.JobRPCClient>(c =>
            {
                c.Address = new Uri(Configuration["GrpcServices:Job"]);
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Authentication");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
                endpoints.MapGrpcService<ProfileGRPCHandler>();
                endpoints.MapGrpcService<CVGRPCHandler>();
            });
        }
    }
}
