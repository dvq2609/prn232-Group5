using System;

namespace backend.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = null!;
        public string? RedirectUrl { get; set; }
        public bool IsRead { get; set; }
        public string CreatedAt { get; set; } = null!;
    }
}
