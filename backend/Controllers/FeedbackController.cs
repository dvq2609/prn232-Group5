using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Feedback;
using backend.DTOs.Feedback;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/feedbacks")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LeaveFeedback([FromBody] CreateFeedbackDto dto)
        {
            try
            {
                var buyerId = GetCurrentUserId();
                var result = await _feedbackService.LeaveFeedbackAsync(dto, buyerId);
                return CreatedAtAction(nameof(GetBySellerProfile), new { sellerId = result.SellerId }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        [HttpPut("{orderId}")]
        [Authorize]
        public async Task<IActionResult> UpdateFeedback(int orderId, [FromBody] CreateFeedbackDto dto)
        {
            try
            {
                dto.OrderId = orderId;
                var buyerId = GetCurrentUserId();
                var result = await _feedbackService.UpdateFeedbackAsync(dto, buyerId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        [HttpDelete("{orderId}")]
        [Authorize]
        public async Task<IActionResult> DeleteFeedback(int orderId)
        {
            try
            {
                var buyerId = GetCurrentUserId();
                await _feedbackService.DeleteFeedbackAsync(orderId, buyerId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetBySellerFeedbacks(int sellerId)
        {
            var feedbacks = await _feedbackService.GetFeedbacksBySellerIdAsync(sellerId);
            return Ok(feedbacks);
        }


        [HttpGet("seller/{sellerId}/profile")]
        public async Task<IActionResult> GetBySellerProfile(int sellerId)
        {
            var profile = await _feedbackService.GetSellerProfileAsync(sellerId);
            return Ok(profile);
        }


        [HttpGet("buyer/{buyerId}")]
        [Authorize]
        public async Task<IActionResult> GetByBuyer(int buyerId)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByBuyerIdAsync(buyerId);
            return Ok(feedbacks);
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
