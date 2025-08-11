using System.Threading.RateLimiting;

namespace Api.Middleware
{
    public static class RateLimitingMiddleware
    {
        public static void AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetTokenBucketLimiter(ipAddress, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100, // máximo de solicitudes permitidas
                        TokensPerPeriod = 100, // recarga cada periodo
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
        }

        public static void UseCustomRateLimiting(this IApplicationBuilder app)
        {
            app.UseRateLimiter();
        }
    }
}
