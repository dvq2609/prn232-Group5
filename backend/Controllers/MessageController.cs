using backend.Services.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("history/{contactId}")]
    public async Task<IActionResult> GetChatHistory(int contactId)
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
        if (!int.TryParse(userIdStr, out int currentUserId))
        {
            return Unauthorized();
        }

        var history = await _messageService.GetChatHistoryAsync(currentUserId, contactId);
        return Ok(history);
    }

    [HttpGet("contacts")]
    public async Task<IActionResult> GetChatContacts()
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
        if (!int.TryParse(userIdStr, out int currentUserId))
        {
            return Unauthorized();
        }

        var contacts = await _messageService.GetChatContactsAsync(currentUserId);
        return Ok(contacts);
    }
}
