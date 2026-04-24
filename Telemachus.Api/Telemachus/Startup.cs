using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Operations.Data;
using Serilog;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Telemachus.Data.Models.Authentication;
using Telemachus.Data.Services.Context;
using Telemachus.Data.Services.DbInitializer;
using Telemachus.Data.Services.Interfaces;
using Telemachus.Data.Services.Services;
using Telemachus.Dependency;
using Telemachus.Middlewares;
using Telemachus.Models;

namespace Telemachus
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = new[]
                    {
                        // Default
                        "text/plain",
                        "text/css",
                        "application/javascript",
                        "text/html",
                        "application/xml",
                        "text/xml",
                        "application/json",
                        "text/json",
                        // Custom
                        "image/x-icon",
                        "image/svg+xml",
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                    };
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.AddControllers();
            services.AddMemoryCache();
            services.AddMvc();
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            DependencyModule.Load(services);
            services.AddHttpClient<ISyncDataService, SyncDataService>(options =>
            {
                options.Timeout = Timeout.InfiniteTimeSpan;
            });

            AddJwtTokenAuthentication(services);
            AddIdentityAuthentication(services, Configuration, Environment);

            // In production, the React files will be served from this directory

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.Configure<IISServerOptions>(options =>
            {
                //options.MaxRequestBodySize = int.MaxValue;

            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.KeepAliveTimeout = Timeout.InfiniteTimeSpan;
                options.Limits.RequestHeadersTimeout = Timeout.InfiniteTimeSpan;
                //options.Limits.MaxRequestBodySize = int.MaxValue;
                //options.Limits.MaxResponseBufferSize = int.MaxValue;
                //options.Limits.MaxRequestBufferSize = int.MaxValue;
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IConfiguration config, IApplicationBuilder app, IWebHostEnvironment env, TelemachusContext telemachusContext)
        {
            var vessel = config.GetSection("VesselDetails").Get<VesselDetails>();

            if (vessel != null)
            {
                if (telemachusContext.Database.CanConnect())
                {
                    if (telemachusContext.Database.GetPendingMigrations().Any())
                        telemachusContext.Database.Migrate();
                }
                else
                {
                    telemachusContext.Database.Migrate();
                    telemachusContext.Database.EnsureCreated();
                }
            }
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();
                dbInitializer.SeedData();
            }

            app.UseResponseCompression();

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Static")),
                RequestPath = "/Static"
            });

            app.UseSpaStaticFiles();

            app.UseSerilogRequestLogging(options =>
            {
                options.Logger = LoggerProvider.CreateAccessLogger(config, env.EnvironmentName);
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("RequestPath", httpContext.Request.Path);
                    diagnosticContext.Set("RequestQueryString", httpContext.Request.QueryString.Value);
                    diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                    diagnosticContext.Set("ClientIP", httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
                };
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();



            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Reports", StringComparison.OrdinalIgnoreCase), appBuilder =>
            {
                appBuilder.UseMiddleware<PasscodeAuthenticationMiddleware>();
            });

            app.UseCors(options =>
            {
                options.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .DisallowCredentials();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
            });

        }

        private void AddJwtTokenAuthentication(IServiceCollection services)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"]));
            var tokenValidationParams = new TokenValidationParameters()
            {
                IssuerSigningKey = signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false
            };
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt => { opt.TokenValidationParameters = tokenValidationParams; });
        }
        private void AddIdentityAuthentication(IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddDbContext<TelemachusContext>(option =>
            {
                var sqlOptions = option.UseSqlServer(config.GetConnectionString("Telemachus"), x => x.UseNetTopologySuite());

                if (env.IsDevelopment())
                {
                    sqlOptions.EnableSensitiveDataLogging();
                }
            });
            services.AddDbContext<OperationsDbContext>(option =>
            {
                var connectionString = config.GetConnectionString("Operations");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var sqlOptions = option.UseSqlServer(config.GetConnectionString("Operations"));
                    if (env.IsDevelopment())
                    {
                        sqlOptions.EnableSensitiveDataLogging();
                    }
                }
            });
            services.AddIdentity<User, IdentityRole>(option =>
                {
                    option.Password.RequireDigit = false;
                    option.Password.RequiredLength = 6;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequireUppercase = false;
                    option.Password.RequireLowercase = false;
                }).AddEntityFrameworkStores<TelemachusContext>()
                .AddDefaultTokenProviders();
        }
    }
}
