using System.Text.Json;

namespace Customers_Demo_Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware>? _logger;

        public ExceptionMiddleware(RequestDelegate next, IApplicationBuilder app)
        {
            _next = next;
            _logger = (ILogger<ExceptionMiddleware>?)app.ApplicationServices.GetService(typeof(ILogger<ExceptionMiddleware>));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await dealException(context, "Operation failed, please check your input content, if there is still a problem, please contact system administrator!");
                _logger?.Log(LogLevel.Error, ex, ex.Message);
            }

        }

        private async Task dealException(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            var stream = context.Response.Body;
            await JsonSerializer.SerializeAsync(stream, new { Status = false, Message = message }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            });
        }

    }
}

