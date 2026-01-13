using ChatApp.Domain.Common;
using System;

namespace ChatApp.Domain.Entities
{
    /// <summary>
    /// Represents a message sent within a conversation.
    /// </summary>
    public class Message : AuditableEntity
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }

        public string Content { get; set; } = null!;

        public bool IsSystemMessage { get; set; }
    }
}
