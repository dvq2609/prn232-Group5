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
       
    }
}