using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Helpers;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace EasyClean.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // Add the class DataContext as a service
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));  // ConnetionString specified in appsetings.Develpment.json 
            ConfigureServices(services); // With the generated services, call the method Configure services present in this class
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            // Add the class DataContext as a service
            services.AddDbContext<DataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection"))); // ConnetionString specified in appsetings.json
            ConfigureServices(services); // With the generated services, call the method Configure services present in this class
        }


        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration to suport role management:
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                // In the next line, we limit the length of TEXT values for MySql databases
                // Follow the link for more information about this:
                // https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/937
                opt.Stores.MaxLengthForKeys = 64; 
                // In dev mode, we set the password requirements to be not so strong
                // so that we can insert easy passowrds for testing
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            services.AddControllers(options =>
            {
                // By aplying this new poilicy, we force every user of our
                // API to, by default, having to be authenticated to use
                // every single endpoint
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            // Make the cors service available so we can use it as middleware in method Configure() 
            // The mehtod config©ure() is where our http request pipline is configured)
            services.AddCors(); 
            // Add automapper as a service so that the properties in our models are automatically mapped
            // with the corresponding properties in our DTOs
            services.AddAutoMapper(typeof(EasyCleanRepository).Assembly);
            // Add service so that we can inject the AuthRepository using the Repository Pattern for authentication
            services.AddScoped<IAuthRepository, AuthRepository>();
            // Add service so that we can inject the EasyCleanRepository using the Repository Pattern for retrieving and deleting users
            // Add service so that we can inject the EasyCleanRepository using the Repository Pattern 
            // With AddScoped<>(), an instance of the service is created once per http-request within the scope
            // this is a compromise between AddSingletone() and AddTransient()
            services.AddScoped<IEasyCleanRepository, EasyCleanRepository>();
            // Specify the schema that will be used for authentication:
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:TokenKey").Value)),
                            ValidateIssuer = false,
                            ValidateAudience = false
                    };
                });
            // add authorization policies to protect our endpoints
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole",
                                  policy => policy.RequireRole("Admin", "ProductDeveloper"));
                options.AddPolicy("RequireBackOfficeRole", 
                                   policy => policy.RequireRole("BackOfficeEmployee", "Admin", "ProductDeveloper"));
                options.AddPolicy("RequireClientManagementRole",
                                    policy => policy.RequireRole("FrontDeskEmployee", "Admin", "ProductDeveloper"));
                options.AddPolicy("RequireEmployeeRole",
                                    policy => policy.RequireRole("BackOfficeEmployee", "FrontDeskEmployee", "Admin", "ProductDeveloper"));
                options.AddPolicy("RequireClientRole",
                                    policy => policy.RequireRole("Client"));
            }
            );
            // add swagger as a service:
            services.AddOpenApiDocument(document =>
            {
                document.Title = "EasyClean Web API";
                document.Description = "This is the Web API for EasyClean";

                // Configure jwt for swagger,
                // This lets adding the jwt to the header
                document.AddSecurity("JWT", Enumerable.Empty<string>(), 
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Cpoy and paste the JWT Token in the fueld 'Value' so:  Bearer {Token JWT}."
                    }
                );

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            // Set up a CORS policy that lets us accept and send back any header, while we are in development
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();

            // Middleware for swagger
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
