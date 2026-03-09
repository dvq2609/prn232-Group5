using System.Security.Claims;
using AutoMapper;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/disputes")]
    [Authorize]
    public class DisputeController : Controller
    {
        private readonly IDisputeService _disputeService;


        public DisputeController(IDisputeService disputeService)
        {

            _disputeService = disputeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDisputes([FromQuery] SieveModel sieveModel)
        {
            var disputes = await _disputeService.GetAllDisputes(sieveModel);
            return Ok(disputes);
        }
        [HttpGet("{id}")]

        public async Task<IActionResult> GetDisputeById([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest("id không hợp lệ");
            }
            var dispute = await _disputeService.GetDisputeById(id);
            return Ok(dispute);
        }
        [HttpGet("buyer/{buyerId}")]
        public async Task<IActionResult> GetDisputesByBuyerId(int buyerId, [FromQuery] SieveModel sieveModel)
        {
            if (buyerId <= 0)
            {
                return BadRequest("buyerId không hợp lệ");
            }
            var disputes = await _disputeService.GetDisputesByBuyerId(buyerId, sieveModel);
            return Ok(disputes);
        }
        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetDisputesBySellerId(int sellerId, [FromQuery] SieveModel sieveModel)
        {
            if (sellerId <= 0)
            {
                return BadRequest("sellerId không hợp lệ");
            }
            var disputes = await _disputeService.GetDisputesBySellerId(sellerId, sieveModel);
            return Ok(disputes);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDispute([FromForm] DisputeCreateDto disputeDto)
        {
            var userIdString = User.FindFirstValue("AccountId");
            if (userIdString == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdString);

            try
            {
                var newDispute = await _disputeService.AddDispute(disputeDto, userId);

                //add image
                if (disputeDto.Images != null && disputeDto.Images.Any())
                {
                    var uploadedImages = new List<DisputeImage>();
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var image in disputeDto.Images)
                    {
                        if (image != null && image.Length > 0)
                        {
                            var allowed = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                            var extension = Path.GetExtension(image.FileName).ToLower();
                            if (!allowed.Contains(extension))
                            {
                                throw new Exception("Invalid file type");
                            }
                            if (image.Length > 5 * 1024 * 1024) throw new Exception("Image must be <5 mb");

                            var fileName = $"{Guid.NewGuid()}{extension}";
                            var filePath = Path.Combine(uploadsFolder, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            var img = new DisputeImage
                            {
                                FileName = fileName,
                                FilePath = "/uploads/" + fileName,
                                FileExtension = extension,
                                FileSizeInBytes = image.Length,
                                DisputeId = newDispute.Id
                            };
                            uploadedImages.Add(img);
                        }
                    }
                    if (uploadedImages.Any())
                    {
                        await _disputeService.AddDisputeImages(uploadedImages);
                    }
                }

                return CreatedAtAction(nameof(CreateDispute), new { newDispute.Id }, disputeDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/seller-response")]
        public async Task<IActionResult> SellerResponse(int id, [FromBody] DisputeSellerResponseDto responseDto)
        {
            var userIdString = User.FindFirstValue("AccountId");
            if (userIdString == null)
            {
                return Unauthorized();
            }
            int sellerId = int.Parse(userIdString);

            try
            {
                var result = await _disputeService.ProcessSellerResponseAsync(id, responseDto, sellerId);
                if (result)
                {
                    return Ok(new { message = "Xử lý phản hồi khiếu nại thành công." });
                }
                return BadRequest("Không thể xử lý phản hồi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/buyer-response")]
        public async Task<IActionResult> BuyerResponse(int id, [FromBody] DisputeBuyerResponseDto responseDto)
        {
            var userIdString = User.FindFirstValue("AccountId");
            if (userIdString == null) return Unauthorized();

            int buyerId = int.Parse(userIdString);
            try
            {
                var result = await _disputeService.ProcessBuyerResponseAsync(id, responseDto, buyerId);
                if (result)
                {
                    return Ok(new { message = "Gửi phản hồi cho người bán thành công." });
                }
                return BadRequest("Không thể gửi phản hồi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
