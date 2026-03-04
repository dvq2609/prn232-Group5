using System.Security.Claims;
using AutoMapper;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/disputes")]
    public class DisputeController : Controller
    {
        private readonly IDisputeService _disputeService;


        public DisputeController(IDisputeService disputeService)
        {

            _disputeService = disputeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDisputes()
        {
            var disputes = await _disputeService.GetAllDisputes();
            return Ok(disputes);
        }
        [HttpGet("buyer/{buyerId}")]
        public async Task<IActionResult> GetDisputesByBuyerId(int buyerId)
        {
            if (buyerId <= 0)
            {
                return BadRequest("buyerId không hợp lệ");
            }
            var disputes = await _disputeService.GetDisputesByBuyerId(buyerId);
            return Ok(disputes);
        }
        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetDisputesBySellerId(int sellerId)
        {
            if (sellerId <= 0)
            {
                return BadRequest("sellerId không hợp lệ");
            }
            var disputes = await _disputeService.GetDisputesBySellerId(sellerId);
            return Ok(disputes);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDispute([FromBody] DisputeCreateDto disputeDto)
        {
            var userIdString = User.FindFirstValue("AccountId");
            if (userIdString == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdString);

            try
            {
                await _disputeService.AddDispute(disputeDto, userId);
                return CreatedAtAction(nameof(CreateDispute), new { disputeDto.OrderId }, disputeDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
