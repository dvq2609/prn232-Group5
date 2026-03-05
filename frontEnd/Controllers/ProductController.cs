using Microsoft.AspNetCore.Mvc;
using frontEnd.Models;
namespace frontEnd.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7290/api/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();
                if (product != null)
                {
                    return View(product);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> BuyNow(int productId, decimal unitPrice)
        {
            // Lấy buyerId từ session (được set khi đăng nhập)
            var accountIdStr = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(accountIdStr) || !int.TryParse(accountIdStr, out int buyerId))
            {
                // Chưa đăng nhập -> redirect về trang login
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient();
            var payload = new
            {
                buyerId,
                productId,
                unitPrice
            };

            var response = await client.PostAsJsonAsync("https://localhost:7290/api/orders/buy-now", payload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "🎉 Đặt hàng thành công! Đơn hàng của bạn đã được xác nhận.";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Đặt hàng thất bại. Vui lòng thử lại.";
            }

            return RedirectToAction("Details", new { id = productId });
        }
    }
}