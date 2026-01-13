using System;

namespace ChatApp.Application.DTOs.User
{
    /// <summary>
    /// Represents a user returned to the client.
    /// </summary>
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsOnline { get; set; }
    }
}
