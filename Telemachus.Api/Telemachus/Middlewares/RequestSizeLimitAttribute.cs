using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Telemachus.Middlewares
{
    public class RequestSizeLimitAttribute : Attribute, IAsyncActionFilter
    {
        private readonly long _sizeLimit;

        public RequestSizeLimitAttribute(long sizeLimit)
        {
            _sizeLimit = sizeLimit;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Check if the request size exceeds the limit
            if (context.HttpContext.Request.ContentLength > _sizeLimit)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                await context.HttpContext.Response.WriteAsync("Request body too large.");
                return; // Skip the execution of the action method
            }

            // Continue with the next filter or action
            await next();
        }
    }






}
