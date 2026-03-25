using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace frontEnd.Helpers
{
    public class RateLimitDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RateLimitDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                // 1. Luôn đính kèm JWT Token nếu có (để RateLimit và Captcha backend nhận ra accountId)
                var token = context.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token) && request.Headers.Authorization == null)
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                // 2. Đính kèm IP thật của Client vào X-Forwarded-For thay vì IP của Docker Frontend Container
                var clientIp = context.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(clientIp) && !request.Headers.Contains("X-Forwarded-For"))
                {
                    request.Headers.Add("X-Forwarded-For", clientIp);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            if ((int)response.StatusCode == 429)
            {
                if (context != null)
                {
                    context.Items["RateLimitExceeded"] = true;
                }
            }

            return response;
        }
    }
}
