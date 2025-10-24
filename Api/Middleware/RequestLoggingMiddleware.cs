using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
            var correlationId = context.TraceIdentifier;

            try
            {
                await _next(context);
                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;

                var userName = context.User?.Identity?.IsAuthenticated == true
                    ? context.User.Identity.Name
                    : "Anonymous";

                _logger.LogInformation(
                    "User {User} made {Method} {Path} with status {StatusCode} in {Elapsed} ms (CorrelationId: {CorrelationId})",
                    userName,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds,
                    correlationId
                );
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "HTTP {Method} {Path} threw an exception after {Elapsed} ms (CorrelationId: {CorrelationId})",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.Elapsed.TotalMilliseconds,
                    correlationId
                );

                throw;
            }
        }
    }
}
