using backend.Models;
using backend.DTOs.Notification;
using Microsoft.EntityFrameworkCore;
using NotificationModel = backend.Models.Notification;

namespace backend.Repositories.Notification
{
    public class NotificationRepository : INotifcationRepository
    {
        private readonly CloneEbayDbContext _context;
        public NotificationRepository(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationDto> CreateNotification(NotificationModel notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return new NotificationDto
            {
                Id = notification.Id,
                Message = notification.Message,
                RedirectUrl = notification.RedirectUrl,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            };
        }

        public async Task<bool> MarkAllAsRead(int userId)
        {
            await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            return true;
        }

        public async Task<bool> MarkAsRead(int id)
        {
            var rowsAffected = await _context.Notifications
                .Where(n => n.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
            return rowsAffected > 0;
        }

        public async Task<List<NotificationDto>> GetAllNotifications(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    RedirectUrl = n.RedirectUrl,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();
        }

        public async Task<NotificationDto?> GetNotificationById(int id)
        {
            return await _context.Notifications
                .Where(n => n.Id == id)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Message = n.Message,
                    RedirectUrl = n.RedirectUrl,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountUnreadNotifications(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }
    }
}