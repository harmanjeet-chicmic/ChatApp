using System;

namespace ChatApp.Application.DTOs.Chat
{
    /// <summary>
    /// Represents a request to send a message in a conversation.
    /// </summary>
    public class SendMessageRequestDto
    {
        public Guid ConversationId { get; set; }
        public string Content { get; set; } = null!;
    }
}
