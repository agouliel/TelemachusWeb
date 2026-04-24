using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Telemachus.Data.Services.Context;

namespace Telemachus.Middlewares
{
    public class PasscodeAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PasscodeAuthenticationMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public PasscodeAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, IWebHostEnvironment environment, IServiceProvider serviceProvider, ILogger<PasscodeAuthenticationMiddleware> logger)
        {
            _next = next;
            _environment = environment;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var isAuthenticated = await IsAuthenticated(context);
            if (!isAuthenticated)
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }

            await _next(context);
        }

        private async Task<bool> IsAuthenticated(HttpContext context)
        {
            var isDevelopment = _environment.EnvironmentName == "Development";
            if (isDevelopment)
            {
                return true;
            }
            var hasVesselSettings = !string.IsNullOrEmpty(_configuration["VesselDetails:Prefix"]);
            var gatewayIp = _configuration["Gateway"];
            var hasVesselGateway = false;
            if (!string.IsNullOrEmpty(gatewayIp))
            {
                var remoteIp = context.Connection.RemoteIpAddress?.ToString();
                if (remoteIp == gatewayIp)
                {
                    hasVesselGateway = true;
                }
            }
            var isInHouse = !hasVesselSettings && !hasVesselGateway;

            if (isInHouse)
                return true;

            var passcode = context.Request.Headers.FirstOrDefault(x => x.Key.ToLower() == "x-passcode").Value.FirstOrDefault();
            if (string.IsNullOrEmpty(passcode)) return false;
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedContext = scope.ServiceProvider.GetRequiredService<TelemachusContext>();
                var claim = context.User?.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier);
                var userId = claim?.Value;
                var validPasscode = await scopedContext.UserPasscodes
                    .Where(x => x.UserId == userId && x.Passcode == passcode)
                    .SingleOrDefaultAsync();
                return validPasscode != null;
            }
        }
    }



}
