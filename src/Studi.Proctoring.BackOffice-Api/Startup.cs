namespace Studi.Proctoring.BackOffice_Api
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Serilog;
    using Studi.Proctoring.BackOffice_Api.Helpers;
    using Studi.Proctoring.BackOffice_Api.Middlewares.Exceptions;
    using Studi.Proctoring.BackOffice_Api.Models;
    using Studi.Proctoring.BackOffice_Api.Repositories;
    using Studi.Proctoring.BackOffice_Api.Repositories.Interfaces;
    using Studi.Proctoring.BackOffice_Api.Services;
    using Studi.Proctoring.BackOffice_Api.Services.Interfaces;
    using Studi.Proctoring.Database.Context;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Startup" />.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the Configuration.
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// The ConfigureServices.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            Log.Information("Start configure services");
            try
            {
                ConfigureServicesWithLoggerActivated(services);
            }
            catch (Exception e)
            {
                Log.Logger.Fatal(e.Message);
            }
        }

        public void ConfigureServicesWithLoggerActivated(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                var allowedCorsOrigins = Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        RequireAudience = false,
                        ValidateAudience = false,
                        ValidateIssuer = false,         // TODO: Issue validation should be perofromed with the Host name the request is sent,
                                                        // HTTP context is not available in scope of IssueValidator
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        RequireSignedTokens = true,
                        ValidateIssuerSigningKey = true,
                    };
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new StudiJwtTokenValidator(new List<SecurityKey>
                    {
                        new SymmetricSecurityKey(
                            System.Convert.FromBase64String(Configuration["AdminJwtInfo:keyString"]))
                    }));
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrators", policy =>
                    policy.RequireRole("Admin"));
                options.AddPolicy("Users", policy =>
                    policy.RequireRole("User", "Admin"));
            });

            //services
            //    .AddAuthentication(BearerAuthenticationHandler.SchemeName)
            //        .AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>(BearerAuthenticationHandler.SchemeName, null);

            //services.AddScoped<IJwtService>(jwt => new JwtService(Configuration.GetSection("PrivateJwtKey").Value));
            //services.AddScoped<IJwtPayloadService, JwtPayloadService>();

            services.AddControllers();
            //.ConfigureApiBehaviorOptions(options =>
            //{
            //    options.InvalidModelStateResponseFactory = context =>
            //    {
            //        var problemDetails = new ValidationProblemDetails(context.ModelState);
            //        var errmsg = problemDetails.Errors.Values.SelectMany(v => v).Distinct().Aggregate(string.Empty, (current, next) => current + next + ". ");
            //        var err = new { Code = "400", Message = errmsg };
            //        return new BadRequestObjectResult(err);
            //    };
            //});

            //services.AddSingleton<IServiceProvider>(sp => sp);

            services.AddDbContext<ProctoringContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ProctoringDbConnection"))
            );

            services.AddControllers().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // TODO: to configure to access mySql serilog logs database
            //services.AddDbContext<MysqlDbContext>(options =>
            //        options.UseMySql(Configuration.GetConnectionString("Mysqlconnection"),
            //        b => b.MigrationsAssembly("Studi.Proctoring.BackOffice_Api.ProctoringLogs"))
            //    );

            services.Configure<PasswordHashConfig>(Configuration.GetSection("PasswordHashConfig"));
            services.Configure<AdminJwtInfo>(Configuration.GetSection("AdminJwtInfo"));

            services.AddHttpClient();

            services.AddAutoMapper(typeof(Startup));

            // Register EF context
            services.AddScoped<IProctoringContext, ProctoringContext>();

            // Register Repositories
            services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
            services.AddScoped<IAdminUserRepository, AdminUserRepository>();
            services.AddScoped<IProctoringRepository, ProctoringRepository>();
            services.AddScoped<IExamSessionRepository, ExamSessionRepository>();
            services.AddScoped<IUserExamSessionRepository, UserExamSessionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
            // Add the image container, on supporting the new containers, this should be replaced with container factory.
            services.AddScoped<IImageRepository, FSImageRepository>(p =>
                new FSImageRepository(Configuration.GetSection("ImageRepository")));

            // Register Services
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IAdminTokenService, AdminTokenService>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IExamSessionService, ExamSessionService>();
            services.AddScoped<IUserExamSessionService, UserExamSessionService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IArchivationService, ArchivationService>();
            services.AddTransient<IHangFireService, HangFireService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGlobalSettingService, GlobalSettingService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Studi.Proctoring.BackOffice_Api", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = @"Please insert JWT.
                                    Enter 'Bearer' [space] and then your token in the text input below.
                                    Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
            });

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Studi.Proctoring.BackOffice_Api", Version = "v1" });
            //#if DEBUG
            //var filePath = Path.Combine(System.AppContext.BaseDirectory, "Studi.Api.Studiforms.xml");
            //c.IncludeXmlComments(filePath);
            //#endif                
            //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //{
            //    Description = @"Please insert JWT.
            //                    Enter 'Bearer' [space] and then your token in the text input below.
            //                    Example: 'Bearer 12345abcdef'",
            //    Name = "Authorization",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.ApiKey,
            //    Scheme = "Bearer"
            //});

            //c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            //{
            //    {
            //      new OpenApiSecurityScheme
            //      {
            //        Reference = new OpenApiReference
            //          {
            //            Type = ReferenceType.SecurityScheme,
            //            Id = "Bearer"
            //          },
            //          Scheme = "oauth2",
            //          Name = "Bearer",
            //          In = ParameterLocation.Header,

            //      },
            //      new List<string>()
            //    }
            //});
            //});
        }

        /// <summary>
        /// The Configure.
        /// </summary>
        /// <param name="app">The app<see cref="IApplicationBuilder"/>.</param>
        /// <param name="env">The env<see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Studi.Proctoring.BackOffice_Api v1"));
            }
            
            app.UseSerilogRequestLogging();

            app.UseExceptionHandlerMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
