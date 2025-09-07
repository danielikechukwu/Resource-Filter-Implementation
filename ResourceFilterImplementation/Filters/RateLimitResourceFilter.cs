using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ResourceFilterImplementation.Filters
{
    public class RateLimitResourceFilter : IAsyncResourceFilter
    {

        private readonly IMemoryCache _memoryCache;

        private readonly int _maxRequestsPerMinute = 5;

        public RateLimitResourceFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // This async method runs before the action method executes
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            // Get the client's IP address as a string; fallback to "unknown" if not found
            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var cacheKey = $"RateLimit_{ip}";

            // Try to get current request count for this IP from cache; default to 0 if not present
            var requestCount = _memoryCache.Get<int?>(cacheKey)  ?? 0;

            if (requestCount >= _maxRequestsPerMinute)
            {
                // Build a Json
                var errorResponse = new
                {
                    Status = 429,
                    Message = "Rate limit exceeded. Try again later."
                };

                // Set the HTTP response immediately with 429 status and the error message
                context.Result = new JsonResult(errorResponse)
                {
                    StatusCode = 429 // HTTP 429 Too Many Requests
                };

                // Short-circuit the request pipeline; do not execute action or other filters
                return;
            }
            else
            {
                _memoryCache.Set(cacheKey, requestCount + 1, TimeSpan.FromMinutes(1));
            }

            // If rate limit not exceeded, proceed with the request execution pipeline
            await next();

        }

    }
}
