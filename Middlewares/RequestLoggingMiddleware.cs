using System.Diagnostics;

namespace VendorRiskScoreAPI.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path;

            _logger.LogInformation("Incoming Request: {Method} {Path}", method, path);

            await _next(context); // request pipeline

            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            _logger.LogInformation("Request Completed: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                method, path, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
