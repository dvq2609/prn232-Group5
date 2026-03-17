using NotificationModel = backend.Models.Notification;
using backend.DTOs.Notification;

namespace backend.Repositories.Notification
{
    public interface INotifcationRepository
    {
        Task<List<NotificationDto>> GetAllNotifications(int userId);
        Task<NotificationDto?> GetNotificationById(int id);
        Task<NotificationDto> CreateNotification(NotificationModel notification);
        Task<bool> MarkAsRead(int id);
        Task<bool> MarkAllAsRead(int userId);
        Task<int> CountUnreadNotifications(int userId);
    }
}