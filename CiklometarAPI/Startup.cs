using CiklometarDAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using CiklometarDAL.Repository;
using CiklometarDAL.Models;
using CiklometarBLL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CiklometarBLL.AutoMapper;
using CiklometarDAL.Repositroy;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using CiklometarBLL;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;

namespace CiklometarAPI
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
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                      ClockSkew = TimeSpan.Zero,
                 };
              });
            services.AddControllers()
                .AddJsonOptions(options => { 
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); 
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
                }); ;
            services.AddDbContext<CiklometarContext>(option =>
            option.UseSqlServer(
                Configuration.GetConnectionString("CiklometarContext"),
                x => x.UseNetTopologySuite()));
            services.AddAutoMapper(typeof(AutoMapping));
            services.AddScoped<IBasicRepository<Role>, BasicRepository<Role>>();
            services.AddScoped<IRepository<User>, Repository<User>>();
            services.AddScoped<IRepository<RefreshToken>, Repository<RefreshToken>>();
            services.AddScoped<IRepository<Organization>, Repository<Organization>>();
            services.AddScoped<IRepository<Requests>, Repository<Requests>>();
            services.AddScoped<IRepository<UserBan>, Repository<UserBan>>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<LoginService>();
            services.AddScoped<BLLFunctions>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IRepository<Location>, Repository<Location>>();
            services.AddScoped<IBasicRepository<StravaTokens>, BasicRepository<StravaTokens>>();
            services.AddScoped<IBasicRepository<Activity>, BasicRepository<Activity>>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IStravaService, StravaService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<UserFactory>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<CiklometarContext>();
            services.AddScoped<ContextDb>();
            services.AddSwaggerGen(c => {
                
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ciklometar API"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
                });
            });
            services.AddCors(setup =>
            {
                setup.AddPolicy("AngularLocalPolicy",
                                config => { config.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CiklometarContext ciklometarContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.Use(async (context, next) =>
                {
                    await next();
                    if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                    {
                        context.Request.Path = "/index.html"; await next();
                    }
                });
            }
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = (c) =>
                {
                    var exception = c.Features.Get<IExceptionHandlerFeature>();
                    var statusCode = exception.Error.GetType().Name switch
                    {
                        "UnauthorizedAccessException" => HttpStatusCode.Unauthorized, 
                        "ArgumentException" => HttpStatusCode.BadRequest,
                        _ => HttpStatusCode.ServiceUnavailable
                    };
                    c.Response.StatusCode = (int)statusCode;
                    var content = Encoding.UTF8.GetBytes($"Error [{exception.Error.Message}]");
                    c.Response.Body.WriteAsync(content, 0, content.Length);
                    return Task.CompletedTask;
                }
            });
            ciklometarContext.Database.Migrate();
            app.UseCors("AngularLocalPolicy");
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}
