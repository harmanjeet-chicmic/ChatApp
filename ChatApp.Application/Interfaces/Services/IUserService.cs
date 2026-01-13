using ChatApp.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync(Guid currentUserId);

        Task UpdateUserStatusAsync(Guid userId, bool isOnline);

        // ✅ ADD THIS
        Task<UserResponseDto> GetByIdAsync(Guid userId);
    }
}
