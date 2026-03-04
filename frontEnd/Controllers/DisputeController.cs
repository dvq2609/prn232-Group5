using frontEnd.Models;
using Microsoft.AspNetCore.Mvc;

namespace frontEnd.Controllers
{
    public class DisputeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DisputeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = HttpContext.Session.GetString("Token");
            var accountId = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://localhost:7290/api/orders/buyer/{accountId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();
                    if (data != null)
                        ViewBag.Orders = data;
                    foreach (var order in data)
                    {
                        Console.WriteLine(order.ProductTitle);
                        Console.WriteLine(order.SellerName);
                        Console.WriteLine(order.OrderDate);
                        Console.WriteLine(order.Status);
                        Console.WriteLine(order.OrderId);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(DisputeViewModel dispute)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = HttpContext.Session.GetString("Token");
                var role = HttpContext.Session.GetString("Role");
                var accountId = HttpContext.Session.GetString("AccountId");
                if (role != "buyer")
                {
                    return RedirectToAction("Login", "Auth");
                }
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                var response = await client.PostAsJsonAsync("https://localhost:7290/api/disputes", dispute);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("BuyerDisputes");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }
        public async Task<IActionResult> BuyerDisputes()
        {
            List<DisputeViewModel> disputes = new List<DisputeViewModel>();
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = HttpContext.Session.GetString("Token");
                var role = HttpContext.Session.GetString("Role");
                var accountId = HttpContext.Session.GetString("AccountId");
                if (role != "buyer")
                {
                    return RedirectToAction("Login", "Auth");
                }
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                var response = await client.GetAsync($"https://localhost:7290/api/disputes/buyer/{accountId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<DisputeViewModel>>();
                    if (data != null)
                    {
                        disputes = data;
                        foreach (var dispute in disputes)
                        {
                            Console.WriteLine(dispute.ProductTitle);
                            Console.WriteLine(dispute.SellerName);
                            Console.WriteLine(dispute.Description);
                            Console.WriteLine(dispute.Status);
                            Console.WriteLine(dispute.OrderId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(disputes);

        }

    }
}
