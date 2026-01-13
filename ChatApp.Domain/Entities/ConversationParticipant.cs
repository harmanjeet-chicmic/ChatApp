using ChatApp.Domain.Common;
using System;

namespace ChatApp.Domain.Entities
{
    public class ConversationParticipant : AuditableEntity
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsMuted { get; set; }
    }
}
