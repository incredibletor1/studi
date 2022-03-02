using Microsoft.AspNetCore.Builder;

namespace Studi.Api.Proctoring.Middlewares.Exceptions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
