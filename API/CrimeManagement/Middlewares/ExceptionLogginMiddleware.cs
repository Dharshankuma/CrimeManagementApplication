using System.Net;
using System.Text.Json;

namespace CrimeManagement.Middlewares
{
    public class ExceptionLogginMiddleware
    {
        public RequestDelegate _next;
        public ILogger<ExceptionLogginMiddleware> _logger;

        public ExceptionLogginMiddleware(RequestDelegate next, ILogger<ExceptionLogginMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Request started | Method : {Method} | Path : {Path} | QueryString : {QueryString} | Timestamp : {Timestamp}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    DateTime.UtcNow);

                await _next(context);

                _logger.LogInformation("Request ended | Method : {Method} | Path : {Path} | QueryString : {QueryString} | StatusCode : {StatusCode} | Timestamp : {Timestamp}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    context.Response.StatusCode,
                    DateTime.UtcNow);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,
                    "Exception Occured | Method : {Method} | Path : {Path} | TraceId : {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);

                await HandleExceptionAsync(context);
            }
        }


        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = "Internal Server Error",
                traceId = context.TraceIdentifier
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
                );
        }
    }
}
