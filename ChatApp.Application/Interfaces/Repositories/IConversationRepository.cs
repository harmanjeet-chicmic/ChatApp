using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines data access operations for conversations.
    /// </summary>
    public interface IConversationRepository
    {
        Task<Conversation?> GetByIdAsync(Guid conversationId);
        Task<Conversation?> GetPrivateConversationAsync(Guid user1Id, Guid user2Id);

        Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId);

        Task AddAsync(Conversation conversation);
        Task UpdateAsync(Conversation conversation);
        
    }
}
