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
                var content = new MultipartFormDataContent();
                if (dispute.OrderId.HasValue)
                    content.Add(new StringContent(dispute.OrderId.Value.ToString()), "OrderId");
                if (!string.IsNullOrEmpty(dispute.Description))
                    content.Add(new StringContent(dispute.Description), "Description");

                if (dispute.Images != null && dispute.Images.Any())
                {
                    foreach (var image in dispute.Images)
                    {
                        var fileContent = new StreamContent(image.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                        content.Add(fileContent, "Images", image.FileName);
                    }
                }

                var response = await client.PostAsync("https://localhost:7290/api/disputes", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("BuyerDisputes");
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Dispute creation failed: {response.StatusCode} - {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during Create Dispute: {ex.Message}");
            }
            return View();
        }
        public async Task<IActionResult> BuyerDisputes()
        {
            List<DisputeResponse> disputes = new List<DisputeResponse>();
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
                    var data = await response.Content.ReadFromJsonAsync<List<DisputeResponse>>();
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
        public async Task<IActionResult> SellerDisputes()
        {
            List<DisputeViewModel> disputes = new List<DisputeViewModel>();
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = HttpContext.Session.GetString("Token");
                var role = HttpContext.Session.GetString("Role");
                var accountId = HttpContext.Session.GetString("AccountId");
                if (role != "seller")
                {
                    return RedirectToAction("Login", "Auth");
                }
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                var response = await client.GetAsync($"https://localhost:7290/api/disputes/seller/{accountId}");
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
                            Console.WriteLine(dispute.SubmittedDate?.ToString("dd/MM/yyyy") ?? "null");

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
