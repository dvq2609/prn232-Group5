using Microsoft.AspNetCore.Mvc;
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
        [ValidateAntiForgeryToken]
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
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    HttpContext.Session.SetString("Token", loginResponse.Token);
                    HttpContext.Session.SetString("Email", loginResponse.Email ?? "");
                    HttpContext.Session.SetString("Role", loginResponse.Role ?? "");
                    HttpContext.Session.SetString("AccountId", loginResponse.AccountId.ToString());
                    return RedirectToAction("Index", "Home");
                }
            }
           
                return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}