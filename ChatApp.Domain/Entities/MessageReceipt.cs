using ChatApp.Domain.Common;
using ChatApp.Domain.Enums;
using System;

namespace ChatApp.Domain.Entities
{
    /// <summary>
    /// Represents the delivery and read state of a message for a user.
    /// </summary>
    public class MessageReceipt : AuditableEntity
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }

        public MessageStatus Status { get; set; }
    }
}
