using System;

namespace ChatApp.Domain.Common
{
    /// <summary>
    /// Base entity that supports auditing and soft deletion.
    /// </summary>
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
