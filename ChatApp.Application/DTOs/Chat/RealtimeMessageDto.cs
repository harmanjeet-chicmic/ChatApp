using System;

namespace ChatApp.Application.DTOs.Chat
{
    public class RealtimeMessageDto
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }

        // ✅ ADD THIS
        public string SenderName { get; set; } = "";

        public string Content { get; set; } = "";
        public DateTime SentAt { get; set; }
    }
}
