using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Order;
using backend.DTOs.Order;
using System.Security.Claims;

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

        /// <summary>
        /// Quick-buy: Tạo order + auto "Delivered" → sẵn sàng review
        /// POST /api/orders/quick-buy
        /// Body: { "productId": 1, "quantity": 1 }
        /// </summary>
        [HttpPost("quick-buy")]
        [Authorize]
        public async Task<IActionResult> QuickBuy([FromBody] QuickBuyDto dto)
        {
            try
            {
                var buyerId = GetCurrentUserId();
                var result = await _orderService.QuickBuyAsync(dto, buyerId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy order theo Id
        /// GET /api/orders/{id}
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Order not found." });
            return Ok(order);
        }

        /// <summary>
        /// Lấy tất cả orders của user đang đăng nhập
        /// GET /api/orders/my-orders
        /// </summary>
        [HttpGet("my-orders")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var buyerId = GetCurrentUserId();
            var orders = await _orderService.GetOrdersByBuyerIdAsync(buyerId);
            return Ok(orders);
        }

        private int GetCurrentUserId()
        {
            var accountIdClaim = User.FindFirst("AccountId");
            if (accountIdClaim == null)
                throw new UnauthorizedAccessException("AccountId claim not found in token.");
            return int.Parse(accountIdClaim.Value);
        }
    }
}