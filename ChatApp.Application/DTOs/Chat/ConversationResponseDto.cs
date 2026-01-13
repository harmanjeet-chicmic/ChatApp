using System;

namespace ChatApp.Application.DTOs.Chat
{
    /// <summary>
    /// Represents a conversation shown in the chat list.
    /// </summary>
    public class ConversationResponseDto
    {
        public Guid ConversationId { get; set; }
        public string DisplayName { get; set; } = null!;
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
    }
}
