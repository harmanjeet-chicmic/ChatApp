using ChatApp.Application.DTOs.Chat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Services
{
    /// <summary>
    /// Defines chat and messaging operations.
    /// </summary>
    public interface IChatService
    {
        Task<Guid> CreateConversationAsync(Guid senderId, Guid receiverId);

        Task<MessageResponseDto> SendMessageAndReturnAsync(
     Guid conversationId,
     Guid senderId,
     string content);


        // Task<IEnumerable<MessageResponseDto>> GetRecentMessagesAsync(
        //     Guid conversationId,
        //     int limit = 10);

        Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId);
        Task<IEnumerable<ConversationListItemDto>> GetUserConversationListAsync(Guid userId);

        Task<IEnumerable<MessageResponseDto>> GetMessagesAsync(
        Guid conversationId,
      DateTime? before,
          int limit);
        Task<Guid> CreateGroupAsync(
         Guid creatorId,
        string groupName,
        IEnumerable<Guid> memberIds);


    }
}
