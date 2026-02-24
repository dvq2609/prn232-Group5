using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using frontEnd.Models;

namespace frontEnd.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7290/api/auth/login", loginViewModel);
            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseViewModel>();
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.JwtToken))
                {
                    HttpContext.Session.SetString("token", loginResponse.JwtToken);
                    HttpContext.Session.SetString("role", loginResponse.Role);
                    HttpContext.Session.SetString("accountID", loginResponse.AccountID.ToString());
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
    }
}