namespace ChatApp.Application.DTOs.Chat
{
    public class ConversationListItemDto
    {
        public Guid ConversationId { get; set; }

        public bool IsGroup { get; set; }

        // PRIVATE CHAT
        public Guid? OtherUserId { get; set; }
        public string? OtherUserName { get; set; }

        // GROUP CHAT
        public string? GroupName { get; set; }

        public string LastMessage { get; set; } = "";
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }

}
