using backend.DTOs.Notification;
using backend.Hubs;
using backend.Models;
using backend.Repositories.Notification;
using Microsoft.AspNetCore.SignalR;

namespace backend.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly INotifcationRepository _repository;
        private readonly IHubContext<ChatHub> _hubContext;

        public NotificationService(INotifcationRepository repository, IHubContext<ChatHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
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

            var created = await _repository.CreateNotification(notification);

            // Gửi SignalR xuống UI ngay lập tức nếu user đang online
            if (created != null)
            {
                var receiverIdStr = userId.ToString();
                if (ChatHub._userConnections.TryGetValue(receiverIdStr, out string connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", created);
                }
            }

            return created!;
        }
    }
}
