using Microsoft.AspNetCore.Mvc;
using frontEnd.Models;

namespace frontEnd.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiBase = "http://backend:8080";

        public FeedbackController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private int? GetAccountId()
        {
            var str = HttpContext.Session.GetString("AccountId");
            if (!string.IsNullOrEmpty(str) && int.TryParse(str, out int id))
                return id;
            return null;
        }

        private string? GetToken()
        {
            return HttpContext.Session.GetString("Token");
        }

        private HttpClient CreateAuthClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }


        [HttpGet]
        public async Task<IActionResult> PurchaseHistory()
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                var response = await client.GetAsync($"{ApiBase}/api/orders/buyer/{accountId}");
                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>() ?? new();
                    return View(orders);
                }
            }
            catch { }

            return View(new List<OrderViewModel>());
        }


        [HttpGet]
        public async Task<IActionResult> LeaveFeedback(int orderId)
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                var response = await client.GetAsync($"{ApiBase}/api/orders/buyer/{accountId}");
                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>() ?? new();
                    var order = orders.FirstOrDefault(o => o.OrderId == orderId);
                    if (order == null)
                    {
                        TempData["ErrorMessage"] = "Order not found.";
                        return RedirectToAction("PurchaseHistory");
                    }
                    if (order.IsCommented == true)
                    {
                        TempData["ErrorMessage"] = "You have already left feedback for this order.";
                        return RedirectToAction("PurchaseHistory");
                    }

                    var model = new CreateFeedbackViewModel
                    {
                        OrderId = orderId,
                        ProductTitle = order.ProductTitle,
                        ProductImage = order.ProductImage,
                        SellerName = order.SellerName,
                        DeliveryOnTime = 5,
                        ExactSame = 5,
                        Communication = 5
                    };
                    return View(model);
                }
            }
            catch { }

            TempData["ErrorMessage"] = "Could not load order information.";
            return RedirectToAction("PurchaseHistory");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveFeedback(CreateFeedbackViewModel model)
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var client = CreateAuthClient();
            var payload = new
            {
                orderId = model.OrderId,
                comment = model.Comment,
                rating = model.Rating,
                deliveryOnTime = model.DeliveryOnTime,
                exactSame = model.ExactSame,
                communication = model.Communication
            };

            try
            {
                var response = await client.PostAsJsonAsync($"{ApiBase}/api/feedbacks", payload);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<FeedbackViewModel>();
                    TempData["FeedbackSellerName"] = result?.SellerName ?? model.SellerName;
                    TempData["FeedbackProductTitle"] = result?.ProductTitle ?? model.ProductTitle;
                    TempData["FeedbackRating"] = model.Rating.ToString();
                    return RedirectToAction("FeedbackSuccess");
                }

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                TempData["ErrorMessage"] = error?.GetValueOrDefault("message") ?? "Failed to submit feedback.";
                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not connect to the server. Please try again.";
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult FeedbackSuccess()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeedback(int orderId)
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                var response = await client.DeleteAsync($"{ApiBase}/api/feedbacks/{orderId}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Feedback deleted successfully.";
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    TempData["ErrorMessage"] = error?.GetValueOrDefault("message") ?? "Failed to delete feedback.";
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not connect to the server.";
            }
            return RedirectToAction("MyFeedbacks");
        }


        [HttpGet]
        public async Task<IActionResult> EditFeedback(int orderId)
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                // Get existing feedback
                var fbResponse = await client.GetAsync($"{ApiBase}/api/feedbacks/buyer/{accountId}");
                if (!fbResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Could not load feedback data.";
                    return RedirectToAction("PurchaseHistory");
                }
                var feedbacks = await fbResponse.Content.ReadFromJsonAsync<List<FeedbackViewModel>>() ?? new();
                var fb = feedbacks.FirstOrDefault(f => f.OrderId == orderId);
                if (fb == null)
                {
                    TempData["ErrorMessage"] = "Feedback not found for this order.";
                    return RedirectToAction("PurchaseHistory");
                }

                var model = new CreateFeedbackViewModel
                {
                    OrderId = orderId,
                    ProductTitle = fb.ProductTitle,
                    ProductImage = fb.ProductImage,
                    SellerName = fb.SellerName,
                    Rating = (int)(fb.AverageRating ?? 5),
                    Comment = fb.Comment,
                    DeliveryOnTime = fb.DeliveryOnTime ?? 5,
                    ExactSame = fb.ExactSame ?? 5,
                    Communication = fb.Communication ?? 5
                };
                ViewData["IsEdit"] = true;
                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not load feedback data.";
                return RedirectToAction("PurchaseHistory");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFeedback(CreateFeedbackViewModel model)
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                ViewData["IsEdit"] = true;
                return View(model);
            }

            var client = CreateAuthClient();
            var payload = new
            {
                orderId = model.OrderId,
                comment = model.Comment,
                rating = model.Rating,
                deliveryOnTime = model.DeliveryOnTime,
                exactSame = model.ExactSame,
                communication = model.Communication
            };

            try
            {
                var response = await client.PutAsJsonAsync($"{ApiBase}/api/feedbacks/{model.OrderId}", payload);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Feedback updated successfully!";
                    return RedirectToAction("PurchaseHistory");
                }

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                TempData["ErrorMessage"] = error?.GetValueOrDefault("message") ?? "Failed to update feedback.";
                ViewData["IsEdit"] = true;
                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not connect to the server. Please try again.";
                ViewData["IsEdit"] = true;
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> MyFeedbacks()
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                var response = await client.GetAsync($"{ApiBase}/api/feedbacks/buyer/{accountId}");
                if (response.IsSuccessStatusCode)
                {
                    var feedbacks = await response.Content.ReadFromJsonAsync<List<FeedbackViewModel>>() ?? new();
                    return View(feedbacks);
                }
            }
            catch { }

            return View(new List<FeedbackViewModel>());
        }


        [HttpGet]
        public async Task<IActionResult> SellerProfile(int sellerId)
        {
            var client = _httpClientFactory.CreateClient();
            var fallback = new SellerFeedbackProfileViewModel
            {
                SellerId = sellerId,
                SellerName = "Unknown Seller"
            };

            try
            {
                var profileTask = client.GetAsync($"{ApiBase}/api/feedbacks/seller/{sellerId}/profile");
                var productsTask = client.GetAsync($"{ApiBase}/api/products");
                await Task.WhenAll(profileTask, productsTask);

                var response = await profileTask;
                var productsResponse = await productsTask;

                var sellerProducts = new List<ProductViewModel>();
                if (productsResponse.IsSuccessStatusCode)
                {
                    var allProducts = await productsResponse.Content.ReadFromJsonAsync<List<ProductViewModel>>() ?? new();
                    sellerProducts = allProducts
                        .Where(p => p.SellerId == sellerId)
                        .ToList();
                }

                if (response.IsSuccessStatusCode)
                {
                    var profile = await response.Content.ReadFromJsonAsync<SellerFeedbackProfileViewModel>();
                    if (profile != null)
                    {
                        profile.SellerProducts = sellerProducts;
                        return View(profile);
                    }
                }

                fallback.SellerProducts = sellerProducts;
            }
            catch { }

            return View(fallback);
        }
    }
}

