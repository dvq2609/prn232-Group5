using Microsoft.AspNetCore.Mvc;

namespace frontEnd.Controllers
{
    public class ChatController : Controller
    {
        private readonly IConfiguration _configuration;

        public ChatController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Token = token;
            ViewBag.CurrentUserId = HttpContext.Session.GetString("AccountId");
            ViewBag.BackendUrl = _configuration["BackendUrl"] ?? "http://localhost:7290";
            
            return View();
        }
    }
}

