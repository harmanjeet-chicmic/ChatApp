using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Configures database mapping for Message entity.
    /// </summary>
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConversationId)
                   .IsRequired();

            builder.Property(x => x.SenderId)
                   .IsRequired();

            builder.Property(x => x.Content)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(x => x.IsSystemMessage)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.IsDeleted)
                   .IsRequired();

            builder.HasIndex(x => x.ConversationId);
        }
    }
}
