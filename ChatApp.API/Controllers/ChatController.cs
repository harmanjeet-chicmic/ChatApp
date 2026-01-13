using ChatApp.Application.Common;
using ChatApp.Application.DTOs.Chat;
using ChatApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ChatApp.API.Extensions;

namespace ChatApp.API.Controllers
{
    /// <summary>
    /// Handles chat and messaging endpoints.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Creates or returns a private conversation.
        /// </summary>
        [HttpPost("conversation")]
        public async Task<IActionResult> CreateConversation([FromQuery] Guid receiverId)
        {
            var senderId = User.GetUserId();

            var conversationId =
                await _chatService.CreateConversationAsync(senderId, receiverId);

            return Ok(SuccessResponse.Create(
                data: new { conversationId },
                message: "Conversation created successfully",
                statusCode: 200));
        }

        /// <summary>
        /// Sends a message to a conversation.
        /// </summary>
        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestDto request)
        {
            var senderId = User.GetUserId();

            // ✅ UPDATED METHOD (NO OTHER SIDE EFFECT)
            await _chatService.SendMessageAndReturnAsync(
                request.ConversationId,
                senderId,
                request.Content);

            return Ok(SuccessResponse.Create<object>(
                data: null,
                message: "Message sent successfully",
                statusCode: 200));
        }

        /// <summary>
        /// Gets paginated messages for a conversation.
        /// </summary>
        [HttpGet("messages/{conversationId}")]
        public async Task<IActionResult> GetMessages(
            Guid conversationId,
            [FromQuery] DateTime? before,
            [FromQuery] int limit = 10)
        {
            var messages = await _chatService.GetMessagesAsync(
                conversationId,
                before,
                limit);

            return Ok(SuccessResponse.Create(
                data: messages,
                message: "Messages fetched successfully",
                statusCode: 200));
        }

        /// <summary>
        /// Marks messages as read for the current user.
        /// </summary>
        [HttpPost("read/{conversationId}")]
        public async Task<IActionResult> MarkAsRead(Guid conversationId)
        {
            var userId = User.GetUserId();

            await _chatService.MarkMessagesAsReadAsync(conversationId, userId);

            return Ok(SuccessResponse.Create<object>(
                data: null,
                message: "Messages marked as read",
                statusCode: 200));
        }

        /// <summary>
        /// Returns all conversations for the current user.
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = User.GetUserId();

            var conversations =
                await _chatService.GetUserConversationListAsync(userId);

            return Ok(SuccessResponse.Create(
                data: conversations,
                message: "Conversations fetched successfully",
                statusCode: 200));
        }

        /// <summary>
        /// Creates a group conversation.
        /// </summary>
        [HttpPost("group")]
        public async Task<IActionResult> CreateGroup(
            [FromBody] CreateGroupRequestDto request)
        {
            var creatorId = User.GetUserId();

            var conversationId = await _chatService.CreateGroupAsync(
                creatorId,
                request.Name,
                request.MemberIds);

            return Ok(SuccessResponse.Create(
                data: new { conversationId },
                message: "Group created successfully",
                statusCode: 201));
        }
    }
}
