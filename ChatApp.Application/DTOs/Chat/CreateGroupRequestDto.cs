using System;
using System.Collections.Generic;

namespace ChatApp.Application.DTOs.Chat
{
    public class CreateGroupRequestDto
    {
        public string Name { get; set; } = null!;
        public List<Guid> MemberIds { get; set; } = new();
    }
}
