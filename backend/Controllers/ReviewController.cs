using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Services.Review;
using backend.DTOs.Review;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            try
            {
                var reviewerId = GetCurrentUserId();
                var result = await _reviewService.CreateReviewAsync(dto, reviewerId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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


        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
            return Ok(reviews);
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var reviews = await _reviewService.GetReviewsByReviewerIdAsync(userId);
            return Ok(reviews);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound(new { message = "Review not found." });
            return Ok(review);
        }


        [HttpGet("product/{productId}/summary")]
        public async Task<IActionResult> GetSummary(int productId)
        {
            var summary = await _reviewService.GetReviewSummaryAsync(productId);
            return Ok(summary);
        }

 
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CreateReviewDto dto)
        {
            try
            {
                var reviewerId = GetCurrentUserId();
                var result = await _reviewService.UpdateReviewAsync(id, dto, reviewerId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var reviewerId = GetCurrentUserId();
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
                var result = await _reviewService.DeleteReviewAsync(id, reviewerId, role);
                if (!result)
                    return NotFound(new { message = "Review not found." });
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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