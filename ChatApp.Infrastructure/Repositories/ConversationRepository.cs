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
    /// Provides data access operations for conversations.
    /// </summary>
    public class ConversationRepository : IConversationRepository
    {
        private readonly ApplicationDbContext _context;

        public ConversationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation?> GetByIdAsync(Guid conversationId)
        {
            return await _context.Conversations
                .Where(x => !x.IsDeleted && x.Id == conversationId)
                .FirstOrDefaultAsync();
        }

        public async Task<Conversation?> GetPrivateConversationAsync(Guid user1Id, Guid user2Id)
        {
            return await _context.Conversations
                .Join(_context.ConversationParticipants,
                      c => c.Id,
                      cp => cp.ConversationId,
                      (c, cp) => new { c, cp })
                .Where(x =>
                    x.c.Type == Domain.Enums.ConversationType.Private &&
                    !x.c.IsDeleted &&
                    (x.cp.UserId == user1Id || x.cp.UserId == user2Id))
                .GroupBy(x => x.c)
                .Where(g => g.Count() == 2)
                .Select(g => g.Key)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId)
        {
            return await _context.ConversationParticipants
                .Where(x => !x.IsDeleted && x.UserId == userId)
                .Join(_context.Conversations,
                      cp => cp.ConversationId,
                      c => c.Id,
                      (cp, c) => c)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Conversation conversation)
        {
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
        }
    }
}
