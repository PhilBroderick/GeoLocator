using System.Net;
using System.Text.Json;

namespace GeoLocator.Web.Middleware
{
    /// <summary>
    /// Serializes base exceptions as JSON response with error code
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                // Make sure it will show full stack trace when running locally
                if (_environment.IsDevelopment())
                {
                    throw;
                }

                SetStatusCode(context, exception);
                SetContentType(context);
                await SetMessage(context, exception);
            }
        }

        private static void SetStatusCode(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = exception switch
            {
                KeyNotFoundException e => (int)HttpStatusCode.NotFound, // 404
                _ => (int)HttpStatusCode.InternalServerError // 500
            };
        }

        private static void SetContentType(HttpContext context)
        {
            if (context.Request.HasJsonContentType())
            {
                context.Response.ContentType = "application/json";
            }
        }

        private static async Task SetMessage(HttpContext context, Exception exception)
        {
            var error = new { message = exception.Message };
            var errorMessage = JsonSerializer.Serialize(error);
            await context.Response.WriteAsync(errorMessage);
        }
    }
}
