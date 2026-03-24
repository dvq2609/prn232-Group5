namespace backend.DTOs.Message;

public class ChatUserDto
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Role { get; set; }
    public MessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public bool LastMessageIsMine { get; set; }
}
