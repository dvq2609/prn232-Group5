using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace frontEnd.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:7290/api/notifications";

        public NotificationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            // For local development, allow self-signed certificates
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            
            var client = new HttpClient(handler);
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var client = CreateClient();
                var response = await client.GetAsync(_apiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return Content(data, "application/json");
                }
                
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[NotificationProxy] API Error: {response.StatusCode} - {error}");
                return StatusCode((int)response.StatusCode, error);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NotificationProxy] Exception: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            try
            {
                var client = CreateClient();
                var response = await client.GetAsync($"{_apiBaseUrl}/unread-count");
                if (response.IsSuccessStatusCode)
                {
                    var count = await response.Content.ReadAsStringAsync();
                    return Ok(int.Parse(count));
                }
                return Ok(0);
            }
            catch
            {
                return Ok(0);
            }
        }

        [HttpPut]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var client = CreateClient();
                var response = await client.PutAsync($"{_apiBaseUrl}/{id}/read", null);
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var client = CreateClient();
                var response = await client.PutAsync($"{_apiBaseUrl}/read-all", null);
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
