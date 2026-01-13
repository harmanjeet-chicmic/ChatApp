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
    /// Provides data access operations for conversation participants.
    /// </summary>
    public class ConversationParticipantRepository : IConversationParticipantRepository
    {
        private readonly ApplicationDbContext _context;

        public ConversationParticipantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<ConversationParticipant> participants)
        {
            await _context.ConversationParticipants.AddRangeAsync(participants);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ConversationParticipant>> GetByConversationIdAsync(Guid conversationId)
        {
            return await _context.ConversationParticipants
                .Where(x => !x.IsDeleted && x.ConversationId == conversationId)
                .ToListAsync();
        }
    }
}
