using System;

namespace ChatApp.Domain.Common
{
    /// <summary>
    /// Represents the base class for all domain entities.
    /// Provides a unique identifier for entity tracking.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }
    }
}
