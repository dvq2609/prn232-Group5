using backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Repositories.Message;

public interface IMessageRepository
{
    Task<IEnumerable<Models.Message>> GetChatHistoryAsync(int userId1, int userId2);
    Task<Models.Message> SaveMessageAsync(Models.Message message);
    Task<IEnumerable<User>> GetChatContactsAsync(int currentUserId);
}
