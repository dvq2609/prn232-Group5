using Microsoft.AspNetCore.Mvc;

namespace frontEnd.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.Token = token;
            ViewBag.CurrentUserId = HttpContext.Session.GetString("AccountId");
            
            return View();
        }
    }
}

