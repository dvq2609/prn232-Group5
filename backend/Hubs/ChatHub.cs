using backend.Services.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;

    // Simple in-memory mapping of user IDs to SignalR connection IDs
    public static readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

    public ChatHub(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            var isNewConnection = !_userConnections.ContainsKey(userId);
            _userConnections[userId] = Context.ConnectionId;
            
            if (isNewConnection)
            {
                await Clients.Others.SendAsync("UserOnline", userId);
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections.TryRemove(userId, out _);
            await Clients.Others.SendAsync("UserOffline", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(int receiverId, string content)
    {
        var senderIdStr = Context.User?.Claims.FirstOrDefault(c => c.Type == "AccountId")?.Value;
        if (int.TryParse(senderIdStr, out int senderId))
        {
            // Save to database
            var savedMessageDto = await _messageService.SaveMessageAsync(senderId, receiverId, content);

            // Send to receiver if online
            var receiverIdStr = receiverId.ToString();
            if (_userConnections.TryGetValue(receiverIdStr, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", savedMessageDto);
            }

            // Also send back to sender so they can update their UI directly if they prefer (optional)
            await Clients.Caller.SendAsync("MessageSent", savedMessageDto);
        }
    }

    public IEnumerable<string> GetOnlineUsers()
    {
        return _userConnections.Keys;
    }
}
