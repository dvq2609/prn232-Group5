using Microsoft.AspNetCore.Mvc;
using frontEnd.Models;

namespace frontEnd.Controllers
{
    public class SellerReviewController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiBase = "http://backend:8080";

        public SellerReviewController(IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> SellerOrders()
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                var response = await client.GetAsync($"{ApiBase}/api/orders/seller/{accountId}");
                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>() ?? new();

                    // Check which orders already have seller reviews
                    var reviewsResponse = await client.GetAsync($"{ApiBase}/api/seller-reviews/seller/{accountId}");
                    var reviews = new List<SellerReviewViewModel>();
                    if (reviewsResponse.IsSuccessStatusCode)
                        reviews = await reviewsResponse.Content.ReadFromJsonAsync<List<SellerReviewViewModel>>() ?? new();

                    var reviewedOrderIds = reviews.Select(r => r.OrderId).ToHashSet();
                    ViewData["ReviewedOrderIds"] = reviewedOrderIds;
                    ViewData["ReviewIdMap"] = reviews.ToDictionary(r => r.OrderId, r => r.Id);

                    return View(orders);
                }
            }
            catch { }

            return View(new List<OrderViewModel>());
        }


        [HttpGet]
        public async Task<IActionResult> LeaveReview(int orderId)
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                var response = await client.GetAsync($"{ApiBase}/api/orders/seller/{accountId}");
                if (response.IsSuccessStatusCode)
                {
                    var orders = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>() ?? new();
                    var order = orders.FirstOrDefault(o => o.OrderId == orderId);
                    if (order == null)
                    {
                        TempData["ErrorMessage"] = "Order not found.";
                        return RedirectToAction("SellerOrders");
                    }

                    var model = new CreateSellerReviewViewModel
                    {
                        OrderId = orderId,
                        BuyerId = order.BuyerId ?? 0,
                        BuyerName = order.BuyerName,
                        ProductTitle = order.ProductTitle,
                        ProductImage = order.ProductImage
                    };
                    return View(model);
                }
            }
            catch { }

            TempData["ErrorMessage"] = "Could not load order information.";
            return RedirectToAction("SellerOrders");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveReview(CreateSellerReviewViewModel model)
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
                buyerId = model.BuyerId,
                comment = model.Comment
            };

            try
            {
                var response = await client.PostAsJsonAsync($"{ApiBase}/api/seller-reviews", payload);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Review for buyer submitted successfully!";
                    return RedirectToAction("SellerOrders");
                }

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                TempData["ErrorMessage"] = error?.GetValueOrDefault("message") ?? "Failed to submit review.";
                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not connect to the server.";
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> MyReviews()
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                var response = await client.GetAsync($"{ApiBase}/api/seller-reviews/seller/{accountId}");
                if (response.IsSuccessStatusCode)
                {
                    var reviews = await response.Content.ReadFromJsonAsync<List<SellerReviewViewModel>>() ?? new();
                    return View(reviews);
                }
            }
            catch { }

            return View(new List<SellerReviewViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> EditReview(int reviewId)
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                var response = await client.GetAsync($"{ApiBase}/api/seller-reviews/seller/{accountId}");
                if (response.IsSuccessStatusCode)
                {
                    var reviews = await response.Content.ReadFromJsonAsync<List<SellerReviewViewModel>>() ?? new();
                    var review = reviews.FirstOrDefault(r => r.Id == reviewId);
                    if (review == null)
                    {
                        TempData["ErrorMessage"] = "Review not found.";
                        return RedirectToAction("MyReviews");
                    }
                    return View(review);
                }
            }
            catch { }

            TempData["ErrorMessage"] = "Could not load review.";
            return RedirectToAction("MyReviews");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(SellerReviewViewModel model)
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            var payload = new { comment = model.Comment };

            try
            {
                var response = await client.PutAsJsonAsync($"{ApiBase}/api/seller-reviews/{model.Id}", payload);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Review updated successfully!";
                    return RedirectToAction("MyReviews");
                }

                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                TempData["ErrorMessage"] = error?.GetValueOrDefault("message") ?? "Failed to update review.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not connect to the server.";
            }

            return RedirectToAction("MyReviews");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var accountId = GetAccountId();
            if (accountId == null)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthClient();
            try
            {
                var response = await client.DeleteAsync($"{ApiBase}/api/seller-reviews/{reviewId}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Review deleted successfully!";
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    TempData["ErrorMessage"] = error?.GetValueOrDefault("message") ?? "Failed to delete review.";
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not connect to the server.";
            }

            return RedirectToAction("MyReviews");
        }
    }
}

