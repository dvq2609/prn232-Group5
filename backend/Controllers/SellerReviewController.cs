using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Services.SellerReview;
using backend.DTOs.SellerReview;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/seller-reviews")]
    public class SellerReviewController : ControllerBase
    {
        private readonly ISellerReviewService _sellerReviewService;

        public SellerReviewController(ISellerReviewService sellerReviewService)
        {
            _sellerReviewService = sellerReviewService;
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LeaveReview([FromBody] CreateSellerReviewDto dto)
        {
            try
            {
                var sellerId = GetCurrentUserId();
                var result = await _sellerReviewService.LeaveSellerReviewAsync(dto, sellerId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetBySellerId(int sellerId)
        {
            var reviews = await _sellerReviewService.GetBySellerIdAsync(sellerId);
            return Ok(reviews);
        }


        [HttpGet("buyer/{buyerId}")]
        public async Task<IActionResult> GetByBuyerId(int buyerId)
        {
            var reviews = await _sellerReviewService.GetByBuyerIdAsync(buyerId);
            return Ok(reviews);
        }

        [HttpPut("{reviewId}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateSellerReviewDto dto)
        {
            try
            {
                var sellerId = GetCurrentUserId();
                var result = await _sellerReviewService.UpdateSellerReviewAsync(reviewId, dto.Comment, sellerId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{reviewId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                var sellerId = GetCurrentUserId();
                await _sellerReviewService.DeleteSellerReviewAsync(reviewId, sellerId);
                return Ok(new { message = "Review deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
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
