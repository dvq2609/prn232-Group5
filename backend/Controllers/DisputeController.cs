using System.Security.Claims;
using AutoMapper;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllDisputes()
        {
            var disputes = await _disputeService.GetAllDisputes();
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

    }
}
