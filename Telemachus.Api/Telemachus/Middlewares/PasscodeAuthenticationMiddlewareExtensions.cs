using Microsoft.AspNetCore.Builder;

namespace Telemachus.Middlewares
{
    public static class PasscodeAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UsePasscodeAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PasscodeAuthenticationMiddleware>();
        }
    }
}
