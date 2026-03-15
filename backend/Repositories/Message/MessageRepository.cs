using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories.Message;

public class MessageRepository : IMessageRepository
{
    private readonly CloneEbayDbContext _context;

    public MessageRepository(CloneEbayDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Models.Message>> GetChatHistoryAsync(int userId1, int userId2)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                        (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task<Models.Message> SaveMessageAsync(Models.Message message)
    {
        message.Timestamp = DateTime.Now;
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<IEnumerable<User>> GetChatContactsAsync(int currentUserId)
    {
        // Get all unique user IDs that the current user has chatted with
        var sentToIds = await _context.Messages
            .Where(m => m.SenderId == currentUserId)
            .Select(m => m.ReceiverId)
            .Distinct()
            .ToListAsync();

        var receivedFromIds = await _context.Messages
            .Where(m => m.ReceiverId == currentUserId)
            .Select(m => m.SenderId)
            .Distinct()
            .ToListAsync();

        var contactIds = sentToIds.Union(receivedFromIds).Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();

        return await _context.Users
            .Where(u => contactIds.Contains(u.Id))
            .ToListAsync();
    }
}
