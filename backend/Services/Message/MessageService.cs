using backend.DTOs.Message;
using backend.Helpers;
using backend.Repositories.Message;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Message;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConfiguration _configuration;
    private readonly string _encryptionKey;

    public MessageService(IMessageRepository messageRepository, IConfiguration configuration)
    {
        _messageRepository = messageRepository;
        _configuration = configuration;
        _encryptionKey = _configuration["Encryption:Key"] ?? string.Empty;
    }

    public async Task<IEnumerable<MessageDto>> GetChatHistoryAsync(int currentUserId, int contactId)
    {
        var messages = await _messageRepository.GetChatHistoryAsync(currentUserId, contactId);
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId ?? 0,
            ReceiverId = m.ReceiverId ?? 0,
            Content = EncryptionHelper.DecryptString(m.Content ?? string.Empty, _encryptionKey),
            Timestamp = m.Timestamp ?? System.DateTime.Now
        });
    }

    public async Task<MessageDto> SaveMessageAsync(int senderId, int receiverId, string content)
    {
        var encryptedContent = EncryptionHelper.EncryptString(content, _encryptionKey);

        var message = new Models.Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = encryptedContent
        };

        var savedMessage = await _messageRepository.SaveMessageAsync(message);

        return new MessageDto
        {
            Id = savedMessage.Id,
            SenderId = savedMessage.SenderId ?? 0,
            ReceiverId = savedMessage.ReceiverId ?? 0,
            Content = content, // Return plain text to SignalR caller
            Timestamp = savedMessage.Timestamp ?? System.DateTime.Now
        };
    }

    public async Task<IEnumerable<ChatUserDto>> GetChatContactsAsync(int currentUserId)
    {
        var contacts = await _messageRepository.GetChatContactsAsync(currentUserId);
        
        var chatUserDtos = new List<ChatUserDto>();
        foreach(var contact in contacts)
        {
            // Get last message between current user and this contact
            var history = await _messageRepository.GetChatHistoryAsync(currentUserId, contact.Id);
            var lastMsg = history.LastOrDefault();
            
            MessageDto? lastMessageDto = null;
            if (lastMsg != null)
            {
                lastMessageDto = new MessageDto
                {
                    Id = lastMsg.Id,
                    SenderId = lastMsg.SenderId ?? 0,
                    ReceiverId = lastMsg.ReceiverId ?? 0,
                    Content = EncryptionHelper.DecryptString(lastMsg.Content ?? string.Empty, _encryptionKey),
                    Timestamp = lastMsg.Timestamp ?? System.DateTime.Now
                };
            }

            chatUserDtos.Add(new ChatUserDto
            {
                Id = contact.Id,
                Username = contact.Username,
                AvatarUrl = contact.AvatarUrl,
                LastMessage = lastMessageDto
            });
        }
        
        return chatUserDtos.OrderByDescending(c => c.LastMessage?.Timestamp).ToList();
    }
}
