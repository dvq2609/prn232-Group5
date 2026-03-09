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
        public async Task<IActionResult> AllDisputes(int page = 1, int pageSize = 5, string status = "all")
        {
            X.PagedList.IPagedList<DisputeResponse> pagedDisputes = null;
            ViewBag.CurrentStatus = status;
            try
            {
                var client = _httpClientFactory.CreateClient();
                var role = HttpContext.Session.GetString("Role");
                var token = HttpContext.Session.GetString("Token");
                var accountId = HttpContext.Session.GetString("AccountId");
                if (role != "admin")
                {
                    return RedirectToAction("Login", "Auth");
                }
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var filterParam = (!string.IsNullOrEmpty(status) && status != "all") ? $"&filters=Status=={status}" : "";
                var response = await client.GetAsync($"https://localhost:7290/api/disputes?page={page}&pageSize={pageSize}&sorts=-DisputeId{filterParam}");
                var statsResponse = await client.GetAsync($"https://localhost:7290/api/disputes?page=1&pageSize=100000");

                if (statsResponse.IsSuccessStatusCode)
                {
                    var statsData = await statsResponse.Content.ReadFromJsonAsync<PagedResult<DisputeResponse>>();
                    if (statsData != null && statsData.Items != null)
                    {
                        ViewBag.Total = statsData.TotalCount;
                        ViewBag.Pending = statsData.Items.Count(d => d.Status?.ToLower() == "pending" || d.Status?.ToLower() == "open");
                        ViewBag.Resolved = statsData.Items.Count(d => d.Status?.ToLower() == "resolved" || d.Status?.ToLower() == "closed");
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    //thay vì trả về list(toàn bộ dữ liệu ) thì trả về model phân trang
                    var data = await response.Content.ReadFromJsonAsync<PagedResult<DisputeResponse>>();
                    if (data != null && data.Items != null)
                    {
                        //chuyển dữ liệu thành pagedlist

                        pagedDisputes = new X.PagedList.StaticPagedList<DisputeResponse>(
                        // danh sách dữ liệu
                        data.Items,
                        // trang hiện tại
                        data.CurrentPage,
                        // số lượng item trong mỗi trang
                        data.PageSize,
                        // tổng số item
                        data.TotalCount);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            pagedDisputes ??= new X.PagedList.StaticPagedList<DisputeResponse>(new List<DisputeResponse>(), page, pageSize, 0);
            return View(pagedDisputes);
        }
        public async Task<IActionResult> BuyerDisputes(int page = 1, int pageSize = 5, string status = "all")
        {
            X.PagedList.IPagedList<DisputeResponse> pagedDisputes = null;
            ViewBag.CurrentStatus = status;
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

                var filterParam = (!string.IsNullOrEmpty(status) && status != "all") ? $"&filters=Status=={status}" : "";
                var response = await client.GetAsync($"https://localhost:7290/api/disputes/buyer/{accountId}?page={page}&pageSize={pageSize}&sorts=-DisputeId{filterParam}");
                var statsResponse = await client.GetAsync($"https://localhost:7290/api/disputes/buyer/{accountId}?page=1&pageSize=100000");

                if (statsResponse.IsSuccessStatusCode)
                {
                    var statsData = await statsResponse.Content.ReadFromJsonAsync<PagedResult<DisputeResponse>>();
                    if (statsData != null && statsData.Items != null)
                    {
                        ViewBag.Total = statsData.TotalCount;
                        ViewBag.Pending = statsData.Items.Count(d => d.Status?.ToLower() == "pending" || d.Status?.ToLower() == "open");
                        ViewBag.Resolved = statsData.Items.Count(d => d.Status?.ToLower() == "resolved" || d.Status?.ToLower() == "closed");
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<PagedResult<DisputeResponse>>();
                    if (data != null && data.Items != null)
                    {
                        pagedDisputes = new X.PagedList.StaticPagedList<DisputeResponse>(data.Items, data.CurrentPage, data.PageSize, data.TotalCount);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            pagedDisputes ??= new X.PagedList.StaticPagedList<DisputeResponse>(new List<DisputeResponse>(), page, pageSize, 0);
            return View(pagedDisputes);
        }
        public async Task<IActionResult> SellerDisputes(int page = 1, int pageSize = 5, string status = "all")
        {
            X.PagedList.IPagedList<DisputeResponse> pagedDisputes = null;
            ViewBag.CurrentStatus = status;
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

                var filterParam = (!string.IsNullOrEmpty(status) && status != "all") ? $"&filters=Status=={status}" : "";
                var response = await client.GetAsync($"https://localhost:7290/api/disputes/seller/{accountId}?page={page}&pageSize={pageSize}&sorts=-DisputeId{filterParam}");
                var statsResponse = await client.GetAsync($"https://localhost:7290/api/disputes/seller/{accountId}?page=1&pageSize=100000");

                if (statsResponse.IsSuccessStatusCode)
                {
                    var statsData = await statsResponse.Content.ReadFromJsonAsync<PagedResult<DisputeResponse>>();
                    if (statsData != null && statsData.Items != null)
                    {
                        ViewBag.Total = statsData.TotalCount;
                        ViewBag.Pending = statsData.Items.Count(d => d.Status?.ToLower() == "pending" || d.Status?.ToLower() == "open");
                        ViewBag.Resolved = statsData.Items.Count(d => d.Status?.ToLower() == "resolved" || d.Status?.ToLower() == "closed");
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<PagedResult<DisputeResponse>>();
                    if (data != null && data.Items != null)
                    {
                        pagedDisputes = new X.PagedList.StaticPagedList<DisputeResponse>(data.Items, data.CurrentPage, data.PageSize, data.TotalCount);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            pagedDisputes ??= new X.PagedList.StaticPagedList<DisputeResponse>(new List<DisputeResponse>(), page, pageSize, 0);
            return View(pagedDisputes);
        }
        public async Task<IActionResult> Details(int id)
        {
            DisputeResponse dispute = new DisputeResponse();
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = HttpContext.Session.GetString("Token");
                var role = HttpContext.Session.GetString("Role");
                var accountId = HttpContext.Session.GetString("AccountId");
                if (role != "buyer" && role != "seller" && role != "admin")
                {
                    return RedirectToAction("Login", "Auth");
                }
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                var response = await client.GetAsync($"https://localhost:7290/api/disputes/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<DisputeResponse>();
                    dispute = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return PartialView("_DisputeDetails", dispute);
        }

        [HttpPost]
        public async Task<IActionResult> SellerResponse(int id, DisputeSellerResponseDto responseDto)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = HttpContext.Session.GetString("Token");
                var role = HttpContext.Session.GetString("Role");
                if (role != "seller")
                {
                    return RedirectToAction("Login", "Auth");
                }
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.PostAsJsonAsync($"https://localhost:7290/api/disputes/{id}/seller-response", responseDto);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Phản hồi khiếu nại thành công.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = "Không thể gửi phản hồi: " + error;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("SellerDisputes");
        }

        [HttpPost]
        public async Task<IActionResult> BuyerResponse(int id, DisputeBuyerResponseDto responseDto)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = HttpContext.Session.GetString("Token");
                var role = HttpContext.Session.GetString("Role");
                if (role != "buyer")
                {
                    return RedirectToAction("Login", "Auth");
                }
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.PostAsJsonAsync($"https://localhost:7290/api/disputes/{id}/buyer-response", responseDto);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Gửi phản hồi cho người bán thành công.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = "Không thể gửi phản hồi: " + error;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("BuyerDisputes");
        }
    }
}
