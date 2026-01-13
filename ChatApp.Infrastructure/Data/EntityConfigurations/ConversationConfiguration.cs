using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Configures database mapping for Conversation entity.
    /// </summary>
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(x => x.GroupId)
                   .IsRequired(false);

            builder.Property(x => x.LastMessageId)
                   .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.IsDeleted)
                   .IsRequired();
        }
    }
}
