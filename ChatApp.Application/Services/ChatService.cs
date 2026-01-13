using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Application.DTOs.Chat;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    /// <summary>
    /// Handles chat, messaging, and conversation business logic.
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMessageReceiptRepository _messageReceiptRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConversationParticipantRepository _conversationParticipantRepository;
        private readonly IGroupRepository _groupRepository;

        /// <summary>
        /// Initializes chat service dependencies.
        /// </summary>
        public ChatService(
            IConversationRepository conversationRepository,
            IMessageRepository messageRepository,
            IMessageReceiptRepository messageReceiptRepository,
            IUserRepository userRepository,
            IConversationParticipantRepository conversationParticipantRepository,
            IGroupRepository groupRepository)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _messageReceiptRepository = messageReceiptRepository;
            _userRepository = userRepository;
            _conversationParticipantRepository = conversationParticipantRepository;
            _groupRepository = groupRepository;
        }

        /// <summary>
        /// Creates or returns an existing private conversation.
        /// </summary>
        public async Task<Guid> CreateConversationAsync(Guid senderId, Guid receiverId)
        {
            var existing =
                await _conversationRepository.GetPrivateConversationAsync(senderId, receiverId);

            if (existing != null)
                return existing.Id;

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Type = ConversationType.Private,
                CreatedAt = DateTime.UtcNow
            };

            await _conversationRepository.AddAsync(conversation);

            await _conversationParticipantRepository.AddRangeAsync(new[]
            {
                new ConversationParticipant
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversation.Id,
                    UserId = senderId,
                    CreatedAt = DateTime.UtcNow
                },
                new ConversationParticipant
                {
                    Id = Guid.NewGuid(),
                    ConversationId = conversation.Id,
                    UserId = receiverId,
                    CreatedAt = DateTime.UtcNow
                }
            });

            return conversation.Id;
        }

        /// <summary>
        /// Saves a message and returns it with sender details.
        /// Used by SignalR and REST.
        /// </summary>
        public async Task<MessageResponseDto> SendMessageAndReturnAsync(
            Guid conversationId,
            Guid senderId,
            string content)
        {
            var conversation =
                await _conversationRepository.GetByIdAsync(conversationId)
                ?? throw new InvalidOperationException("Conversation not found.");

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                IsSystemMessage = false
            };

            await _messageRepository.AddAsync(message);

            conversation.LastMessageId = message.Id;
            await _conversationRepository.UpdateAsync(conversation);

            var participants =
                await _conversationParticipantRepository.GetByConversationIdAsync(conversationId);

            await _messageReceiptRepository.AddRangeAsync(
                participants
                    .Where(p => p.UserId != senderId)
                    .Select(p => new MessageReceipt
                    {
                        Id = Guid.NewGuid(),
                        MessageId = message.Id,
                        UserId = p.UserId,
                        Status = MessageStatus.Sent,
                        CreatedAt = DateTime.UtcNow
                    })
            );

            var sender = await _userRepository.GetByIdAsync(senderId);

            return new MessageResponseDto
            {
                MessageId = message.Id,
                SenderId = senderId,
                SenderName = sender!.FullName,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                IsSystemMessage = false
            };
        }

        /// <summary>
        /// Fetches paginated messages for a conversation.
        /// </summary>
        public async Task<IEnumerable<MessageResponseDto>> GetMessagesAsync(
            Guid conversationId,
            DateTime? before,
            int limit)
        {
            var messages =
                await _messageRepository.GetMessagesBeforeAsync(conversationId, before, limit);

            var users =
                await _userRepository.GetByIdsAsync(messages.Select(m => m.SenderId).Distinct());

            return messages.Select(m =>
            {
                var sender = users.First(u => u.Id == m.SenderId);

                return new MessageResponseDto
                {
                    MessageId = m.Id,
                    SenderId = m.SenderId,
                    SenderName = sender.FullName,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    IsSystemMessage = m.IsSystemMessage
                };
            });
        }

        /// <summary>
        /// Marks all unread messages as read for a user.
        /// </summary>
        public async Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId)
        {
            var receipts =
                await _messageReceiptRepository.GetUnreadReceiptsAsync(conversationId, userId);

            foreach (var receipt in receipts)
            {
                receipt.Status = MessageStatus.Read;
                receipt.UpdatedAt = DateTime.UtcNow;
            }

            await _messageReceiptRepository.UpdateRangeAsync(receipts);
        }

        /// <summary>
        /// Returns conversation list for sidebar (private + groups).
        /// </summary>
        public async Task<IEnumerable<ConversationListItemDto>>
    GetUserConversationListAsync(Guid userId)
        {
            var conversations =
                await _conversationRepository.GetUserConversationsAsync(userId);

            var lastMessages =
                await _messageRepository.GetLastMessagesForConversationsAsync(
                    conversations.Select(c => c.Id));

            var lastMessageMap = lastMessages
                .GroupBy(m => m.ConversationId)
                .ToDictionary(g => g.Key, g => g.First());

            var result = new List<ConversationListItemDto>();

            foreach (var c in conversations)
            {
                var unread =
                    (await _messageReceiptRepository
                        .GetUnreadReceiptsAsync(c.Id, userId)).Count();

                lastMessageMap.TryGetValue(c.Id, out var last);

                // ---------------- GROUP ----------------
                if (c.Type == ConversationType.Group)
                {
                    var group = await _groupRepository.GetByIdAsync(c.GroupId!.Value);
                    if (group == null) continue;

                    result.Add(new ConversationListItemDto
                    {
                        ConversationId = c.Id,
                        IsGroup = true,
                        GroupName = group.Name,
                        LastMessage = last?.Content ?? "",
                        LastMessageAt = last?.CreatedAt,
                        UnreadCount = unread
                    });

                    continue;
                }

                // ---------------- PRIVATE ----------------
                var participants =
                    await _conversationParticipantRepository.GetByConversationIdAsync(c.Id);

                var other = participants.FirstOrDefault(p => p.UserId != userId);
                if (other == null)
                    continue;

                var user = await _userRepository.GetByIdAsync(other.UserId);
                if (user == null)
                    continue;

                result.Add(new ConversationListItemDto
                {
                    ConversationId = c.Id,
                    IsGroup = false,
                    OtherUserId = user.Id,
                    OtherUserName = user.FullName,
                    LastMessage = last?.Content ?? "",
                    LastMessageAt = last?.CreatedAt,
                    UnreadCount = unread
                });
            }

            return result
                .OrderByDescending(x => x.LastMessageAt ?? DateTime.MinValue);
        }


        /// <summary>
        /// Creates a group conversation and assigns admin.
        /// </summary>
        public async Task<Guid> CreateGroupAsync(
            Guid creatorId,
            string groupName,
            IEnumerable<Guid> memberIds)
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = groupName,
                CreatedBy = creatorId,
                CreatedAt = DateTime.UtcNow
            };

            await _groupRepository.AddAsync(group);

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Type = ConversationType.Group,
                GroupId = group.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _conversationRepository.AddAsync(conversation);

            await _conversationParticipantRepository.AddRangeAsync(
                memberIds
                    .Append(creatorId)
                    .Distinct()
                    .Select(id => new ConversationParticipant
                    {
                        Id = Guid.NewGuid(),
                        ConversationId = conversation.Id,
                        UserId = id,
                        IsAdmin = id == creatorId,
                        CreatedAt = DateTime.UtcNow
                    })
            );

            return conversation.Id;
        }
    }
}
