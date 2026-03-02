using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST /api/orders/buy-now
        [HttpPost("buy-now")]
        public async Task<IActionResult> BuyNow([FromBody] BuyNowRequest request)
        {
            if (request.BuyerId <= 0 || request.ProductId <= 0 || request.UnitPrice <= 0)
                return BadRequest("Invalid request data.");

            var order = await _orderService.BuyNowAsync(request.BuyerId, request.ProductId, request.UnitPrice);

            return CreatedAtAction(nameof(BuyNow), new
            {
                id = order.Id,
                status = order.Status,
                orderDate = order.OrderDate,
                totalPrice = order.TotalPrice
            });
        }
    }

    public class BuyNowRequest
    {
        public int BuyerId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
