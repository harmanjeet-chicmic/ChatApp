using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repositories
{
    /// <summary>
    /// Provides data access operations for message delivery and read receipts.
    /// </summary>
    public class MessageReceiptRepository : IMessageReceiptRepository
    {   
        private readonly ApplicationDbContext _context;

        public MessageReceiptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<MessageReceipt> receipts)
        {
            await _context.MessageReceipts.AddRangeAsync(receipts);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MessageReceipt>> GetUnreadReceiptsAsync(Guid conversationId, Guid userId)
        {
            return await _context.MessageReceipts
                .Join(_context.Messages,
                      receipt => receipt.MessageId,
                      message => message.Id,
                      (receipt, message) => new { receipt, message })
                .Where(x =>
                    !x.receipt.IsDeleted &&
                    !x.message.IsDeleted &&
                    x.receipt.UserId == userId &&
                    x.receipt.Status != Domain.Enums.MessageStatus.Read &&
                    x.message.ConversationId == conversationId)
                .Select(x => x.receipt)
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<MessageReceipt> receipts)
        {
            _context.MessageReceipts.UpdateRange(receipts);
            await _context.SaveChangesAsync();
        }
    }
}
