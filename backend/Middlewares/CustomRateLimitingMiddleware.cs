using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;

namespace backend.Middlewares
{
    public class CustomRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        // Giới hạn max 150 request trong 1 phút (Để chống Tool, DDoS)
        private readonly int _maxRequests = 5;
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

        public CustomRateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Bỏ qua chặn với API verify captcha hoặc signalR hub
            if (path != null && (path.Contains("/api/captcha") || path.Contains("/chathub")))
            {
                await _next(context);
                return;
            }

            // Ưu tiên đếm theo AccountId từ JWT Token, nếu chưa đăng nhập thì dùng IP address
            var accountId = context.User.FindFirst("AccountId")?.Value;
            var clientId = !string.IsNullOrEmpty(accountId) ? $"user_{accountId}" : $"ip_{context.Connection.RemoteIpAddress?.ToString()}";

            var cacheKey = $"RateLimit_{clientId}";

            var currentHitCount = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _timeWindow;
                return 0;
            });

            if (currentHitCount >= _maxRequests)
            {
                // Ngắt kết nối, không đẩy response vào controller nữa
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";
                var responseObj = new
                {
                    message = "Phát hiện tần suất truy cập cao bất thường, vui lòng xác minh Captcha để tiếp tục.",
                    requireCaptcha = true
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(responseObj));
                return;
            }

            _cache.Set(cacheKey, currentHitCount + 1, _timeWindow);

            await _next(context);
        }
    }

    public static class CustomRateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomRateLimitingMiddleware>();
        }
    }
}
