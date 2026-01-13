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
    /// Provides data access operations for messages.
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetRecentMessagesAsync(Guid conversationId, int limit)
        {
            return await _context.Messages
                .Where(x => !x.IsDeleted && x.ConversationId == conversationId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(limit)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<Message>> GetLastMessagesForConversationsAsync(
    IEnumerable<Guid> conversationIds)
        {
            return await _context.Messages
                .Where(m =>
                    conversationIds.Contains(m.ConversationId) &&
                    !m.IsDeleted)
                .GroupBy(m => m.ConversationId)
                .Select(g => g
                    .OrderByDescending(m => m.CreatedAt)
                    .First())
                .ToListAsync();
        }
        public async Task<IEnumerable<Message>> GetMessagesBeforeAsync(
     Guid conversationId,
     DateTime? before,
     int limit)
        {
            var query = _context.Messages
                .Where(m => !m.IsDeleted && m.ConversationId == conversationId);

            if (before.HasValue)
            {
                query = query.Where(m => m.CreatedAt < before.Value);
            }

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Take(limit)
                .OrderBy(m => m.CreatedAt) // IMPORTANT: oldest → newest
                .ToListAsync();
        }

    }
}
