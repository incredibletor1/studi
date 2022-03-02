using Microsoft.AspNetCore.Builder;

namespace Studi.Proctoring.BackOffice_Api.Middlewares.Exceptions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
