using ChatApp.API.Extensions;
using ChatApp.API.SignalR;
using ChatApp.Application.DTOs.Chat;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ChatApp.API.Hubs
{
    /// <summary>
    /// SignalR hub responsible for handling all real-time communication
    /// related to chats, including presence tracking, message delivery,
    /// and conversation group management.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly UserPresenceTracker _presenceTracker;
        private readonly IConversationRepository _conversationRepository;
        private readonly IConversationParticipantRepository _conversationParticipantRepository;

        /// <summary>
        /// Initializes a new instance of <see cref="ChatHub"/> with all required
        /// services for messaging, presence tracking, and conversation management.
        /// </summary>
        public ChatHub(
            IChatService chatService,
            IUserService userService,
            UserPresenceTracker presenceTracker,
            IConversationRepository conversationRepository,
            IConversationParticipantRepository conversationParticipantRepository)
        {
            _chatService = chatService;
            _userService = userService;
            _presenceTracker = presenceTracker;
            _conversationRepository = conversationRepository;
            _conversationParticipantRepository = conversationParticipantRepository;
        }

        // ============================================================
        // CONNECTION LIFECYCLE
        // ============================================================

        /// <summary>
        /// Executes when a client establishes a new SignalR connection.
        /// This method:
        /// 1. Registers the user's connection in the presence tracker.
        /// 2. Updates the user's online status if this is their first active connection.
        /// 3. Notifies all other connected users that this user is now online.
        /// 4. Automatically joins the user to all their existing conversations
        ///    so they can immediately receive real-time messages.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Guid userId = Context.User!.GetUserId();

            bool isFirstConnection =
                _presenceTracker.UserConnected(userId, Context.ConnectionId);

            if (isFirstConnection)
            {
                await _userService.UpdateUserStatusAsync(userId, true);
                await Clients.Others.SendAsync("UserOnline", userId);
            }

            var conversations =
                await _conversationRepository.GetUserConversationsAsync(userId);

            foreach (var conversation in conversations)
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    conversation.Id.ToString());
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Executes when a SignalR connection is closed or lost.
        /// This method:
        /// 1. Removes the connection from the presence tracker.
        /// 2. If this was the user's last active connection, marks the user as offline.
        /// 3. Broadcasts the offline status to all remaining connected users.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Guid userId = Context.User!.GetUserId();

            bool isLastConnection =
                _presenceTracker.UserDisconnected(userId, Context.ConnectionId);

            if (isLastConnection)
            {
                await _userService.UpdateUserStatusAsync(userId, false);
                await Clients.Others.SendAsync("UserOffline", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Receives periodic heartbeat signals from the client.
        /// This keeps the user's connection alive in the presence tracker
        /// and prevents accidental offline states caused by network delays
        /// or browser tab suspension.
        /// </summary>
        public Task Heartbeat()
        {
            Guid userId = Context.User!.GetUserId();
            _presenceTracker.Refresh(userId, Context.ConnectionId);
            return Task.CompletedTask;
        }

        // ============================================================
        // GROUP MEMBERSHIP
        // ============================================================

        /// <summary>
        /// Adds the current SignalR connection to the specified conversation group.
        /// This ensures the user receives real-time messages for that conversation.
        /// Additionally, all unread messages for this user in the conversation
        /// are marked as read when the chat is opened.
        /// </summary>
        /// <param name="conversationId">The unique identifier of the conversation to join.</param>
        public async Task JoinConversation(Guid conversationId)
        {
            var userId = Context.User!.GetUserId();

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                conversationId.ToString());

            await _chatService.MarkMessagesAsReadAsync(conversationId, userId);
        }

        // ============================================================
        // MESSAGING
        // ============================================================

        /// <summary>
        /// Sends a message to a conversation.
        /// This method:
        /// 1. Persists the message in the database.
        /// 2. Ensures all participants are connected to the SignalR group.
        /// 3. Broadcasts the message in real time to all members of the conversation.
        /// </summary>
        /// <param name="conversationId">The conversation in which the message is sent.</param>
        /// <param name="content">The text content of the message.</param>
        public async Task SendMessage(Guid conversationId, string content)
        {
            Guid senderId = Context.User!.GetUserId();

            var saved =
                await _chatService.SendMessageAndReturnAsync(
                    conversationId,
                    senderId,
                    content);

            await EnsureParticipantsInGroup(conversationId);

            var realtime = new RealtimeMessageDto
            {
                ConversationId = conversationId,
                SenderId = saved.SenderId,
                SenderName = saved.SenderName,
                Content = saved.Content,
                SentAt = saved.CreatedAt
            };

            await Clients
                .Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", realtime);
        }

        // ============================================================
        // PRIVATE HELPERS
        // ============================================================

        /// <summary>
        /// Ensures that all users participating in a conversation
        /// have all of their active SignalR connections added to
        /// the corresponding SignalR group. This guarantees that
        /// every participant receives real-time messages regardless
        /// of how many devices or browser tabs they are connected from.
        /// </summary>
        /// <param name="conversationId">The conversation whose participants should be synchronized.</param>
        private async Task EnsureParticipantsInGroup(Guid conversationId)
        {
            var participants =
                await _conversationParticipantRepository
                    .GetByConversationIdAsync(conversationId);

            foreach (var participant in participants)
            {
                var connections =
                    _presenceTracker.GetConnections(participant.UserId);

                foreach (var connectionId in connections)
                {
                    await Groups.AddToGroupAsync(
                        connectionId,
                        conversationId.ToString());
                }
            }
        }
    }
}
