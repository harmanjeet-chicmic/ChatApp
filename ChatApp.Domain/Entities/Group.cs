using ChatApp.Domain.Common;
using System;

namespace ChatApp.Domain.Entities
{
    /// <summary>
    /// Represents a group chat with shared metadata.
    /// </summary>
    public class Group : AuditableEntity
    {
        public string Name { get; set; } = null!;
        public Guid CreatedBy { get; set; }
    }
}
