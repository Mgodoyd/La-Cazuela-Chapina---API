using System.Diagnostics;

namespace Api.Middleware
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
            var request = context.Request;

            _logger.LogInformation("Incoming Request: {Method} {Path}", request.Method, request.Path);

            await _next(context);

            stopwatch.Stop();
            _logger.LogInformation("Response {StatusCode} for {Method} {Path} in {Elapsed} ms",
                context.Response.StatusCode,
                request.Method,
                request.Path,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
