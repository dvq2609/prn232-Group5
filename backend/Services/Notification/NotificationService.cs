using backend.DTOs.Notification;
using backend.Models;
using backend.Repositories.Notification;

namespace backend.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly INotifcationRepository _repository;

        public NotificationService(INotifcationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            return await _repository.GetAllNotifications(userId);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _repository.CountUnreadNotifications(userId);
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            return await _repository.MarkAsRead(id);
        }

        public async Task<bool> MarkAllReadAsync(int userId)
        {
            return await _repository.MarkAllAsRead(userId);
        }

        public async Task<NotificationDto> SendNotificationAsync(int userId, string message, string? redirectUrl)
        {
            var notification = new backend.Models.Notification
            {
                UserId = userId,
                Message = message,
                RedirectUrl = redirectUrl,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            return await _repository.CreateNotification(notification);
        }
    }
}
