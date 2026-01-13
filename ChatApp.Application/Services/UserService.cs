using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Application.DTOs.User;
using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync(Guid currentUserId)
        {
            var users = await _userRepository.GetAllExceptAsync(currentUserId);

            return users.Select(user => new UserResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsOnline = user.Status == UserStatus.Online
            });
        }

        public async Task UpdateUserStatusAsync(Guid userId, bool isOnline)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            user.Status = isOnline ? UserStatus.Online : UserStatus.Offline;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        // ✅ ADD THIS
        public async Task<UserResponseDto> GetByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            return new UserResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsOnline = user.Status == UserStatus.Online
            };
        }
    }
}
