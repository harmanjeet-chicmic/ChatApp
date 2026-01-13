using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.EntityConfigurations
{
    /// <summary>
    /// Configures database mapping for MessageReceipt entity.
    /// </summary>
    public class MessageReceiptConfiguration 
        : IEntityTypeConfiguration<MessageReceipt>
    {
        public void Configure(EntityTypeBuilder<MessageReceipt> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MessageId)
                   .IsRequired();

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.IsDeleted)
                   .IsRequired();

            builder.HasIndex(x => new { x.MessageId, x.UserId })
                   .IsUnique();
        }
    }
}
