using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Repositories
{
    /// <summary>
    /// Defines data access operations for messages.
    /// </summary>
    public interface IMessageRepository
    {
        Task AddAsync(Message message);

        Task<IEnumerable<Message>> GetRecentMessagesAsync(Guid conversationId, int limit);
        Task<IEnumerable<Message>> GetLastMessagesForConversationsAsync(
    IEnumerable<Guid> conversationIds);
        Task<IEnumerable<Message>> GetMessagesBeforeAsync(
   Guid conversationId,
   DateTime? before,
   int limit);

    }
}
