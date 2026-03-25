using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public CaptchaController(IMemoryCache cache, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("verify-captcha")]
        public async Task<IActionResult> VerifyCaptcha([FromBody] CaptchaVerifyRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new { success = false, message = "Captcha Token is missing!" });
            }

            var secretKey = _configuration["GoogleRecaptcha:SecretKey"];
            var client = _httpClientFactory.CreateClient();

            // Check verified token với backend của Google
            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={request.Token}", null);
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(jsonString);

            if (result.GetProperty("success").GetBoolean())
            {
                // Xác minh thành công, tìm định danh của request và gỡ limit
                var accountId = User.FindFirst("AccountId")?.Value;
                var clientId = !string.IsNullOrEmpty(accountId) ? $"user_{accountId}" : $"ip_{HttpContext.Connection.RemoteIpAddress?.ToString()}";
                var cacheKey = $"RateLimit_{clientId}";

                // Cho đi tiếp bằng việc huỷ cache
                _cache.Remove(cacheKey);

                return Ok(new { success = true, message = "Xác thực an toàn." });
            }

            return BadRequest(new { success = false, message = "Mã xác thực không đúng hoặc đã hết hạn." });
        }
    }

    public class CaptchaVerifyRequest
    {
        public string? Token { get; set; }
    }
}
