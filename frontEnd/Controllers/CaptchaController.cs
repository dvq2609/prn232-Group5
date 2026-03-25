using Microsoft.AspNetCore.Mvc;

namespace frontEnd.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiBase = "http://backend:8080";

        public CaptchaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Verify([FromBody] CaptchaTokenRequest req)
        {
            var client = _httpClientFactory.CreateClient();
            
            // Đính kèm token đăng nhập để Backend nhận diện AccountId
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.PostAsJsonAsync($"{ApiBase}/api/captcha/verify-captcha", new { token = req.Token });
            var result = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, result);
        }
    }

    public class CaptchaTokenRequest 
    { 
        public string? Token { get; set; } 
    }
}
