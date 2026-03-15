using backend.DTOs.Message;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services.Message;

public interface IMessageService
{
    Task<IEnumerable<MessageDto>> GetChatHistoryAsync(int currentUserId, int contactId);
    Task<MessageDto> SaveMessageAsync(int senderId, int receiverId, string content);
    Task<IEnumerable<ChatUserDto>> GetChatContactsAsync(int currentUserId);
}
