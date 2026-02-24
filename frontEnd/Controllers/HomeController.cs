using frontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace frontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var products = new List<ProductViewModel>();
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://localhost:7290/api/products");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<ProductViewModel>>();
                    if (data != null) products = data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Không thể kết nối tới API: {Message}", ex.Message);
                ViewData["ApiError"] = "Không thể tải sản phẩm. API đang offline.";
            }
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
