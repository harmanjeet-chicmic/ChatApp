using ChatApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines data access operations for conversation participants.
    /// </summary>
    public interface IConversationParticipantRepository
    {
        Task AddRangeAsync(IEnumerable<ConversationParticipant> participants);
        Task<IEnumerable<ConversationParticipant>> GetByConversationIdAsync(Guid conversationId);
    }
}
