using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Configures database mapping for ConversationParticipant entity.
    /// </summary>
    public class ConversationParticipantConfiguration 
        : IEntityTypeConfiguration<ConversationParticipant>
    {
        public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConversationId)
                   .IsRequired();

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.IsAdmin)
                   .IsRequired();

            builder.Property(x => x.IsMuted)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.IsDeleted)
                   .IsRequired();

            builder.HasIndex(x => new { x.ConversationId, x.UserId })
                   .IsUnique();
        }
    }
}
