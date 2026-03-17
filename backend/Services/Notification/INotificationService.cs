using backend.DTOs.Notification;
using backend.Models;

namespace backend.Services.Notification
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int id);
        Task<bool> MarkAllReadAsync(int userId);
        Task<NotificationDto> SendNotificationAsync(int userId, string message, string? redirectUrl);
    }
}
