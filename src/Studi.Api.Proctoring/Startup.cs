namespace Studi.Api.Proctoring
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
    using Studi.Api.Proctoring.Helpers;
    using Studi.Api.Proctoring.Middlewares.Exceptions;
    using Studi.Api.Proctoring.Models;
    using Studi.Api.Proctoring.Repositories;
    using Studi.Api.Proctoring.Services;
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
                        //IssuerSigningKey = new SymmetricSecurityKey(
                        //    System.Convert.FromBase64String(Configuration["JwtInfo:keyString"]))
                    };
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new StudiJwtTokenValidator(new List<SecurityKey>
                    {
                        new SymmetricSecurityKey(
                            System.Convert.FromBase64String(Configuration["JwtInfo:keyString"])),
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
            //        b => b.MigrationsAssembly("Studi.Api.Proctoring.ProctoringLogs"))
            //    );

            services.Configure<JwtInfo>(Configuration.GetSection("JwtInfo"));
            services.Configure<FeatureFlipping>(Configuration.GetSection("FeatureFlipping"));

            services.AddHttpClient();

            services.AddAutoMapper(typeof(Startup));

            // Register EF context
            services.AddScoped<IProctoringContext, ProctoringContext>();

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProctoringRepository, ProctoringRepository>();
            services.AddScoped<IExamSessionRepository, ExamSessionRepository>();
            services.AddScoped<IUserExamSessionRepository, UserExamSessionRepository>();
            services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
            // Add the image container, on supporting the new containers, this should be replaced by container factory.
            services.AddScoped<IImageRepository, FSImageRepository>(p =>
                new FSImageRepository(Configuration.GetSection("ImageRepository")));

            // Register Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProctoringService, ProctoringService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IExamSessionService, ExamSessionService>();
            services.AddScoped<IUserExamSessionService, UserExamSessionService>();
            services.AddScoped<ILmsApiClient, LmsApiClient>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddMemoryCache();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IGlobalSettingService, GlobalSettingService>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Studi.Api.Proctoring", Version = "v1" });
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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Studi.Api.Proctoring v1"));
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
