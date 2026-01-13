using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines data access operations for message delivery and read states.
    /// </summary>
    public interface IMessageReceiptRepository
    {
        Task AddRangeAsync(IEnumerable<MessageReceipt> receipts);

        Task<IEnumerable<MessageReceipt>> GetUnreadReceiptsAsync(Guid conversationId, Guid userId);

        Task UpdateRangeAsync(IEnumerable<MessageReceipt> receipts);
    }
}
