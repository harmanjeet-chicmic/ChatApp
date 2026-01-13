using ChatApp.Application.Common;
using ChatApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ChatApp.API.Extensions;

namespace ChatApp.API.Controllers
{
    /// <summary>
    /// Handles user-related endpoints.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var currentUserId = User.GetUserId();

            var users = await _userService.GetAllUsersAsync(currentUserId);

            return Ok(SuccessResponse.Create(
                data: users,
                message: "Users fetched successfully",
                statusCode: 200));
        }

        [HttpPost("status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] bool isOnline)
        {
            var userId = User.GetUserId();

            await _userService.UpdateUserStatusAsync(userId, isOnline);

            return Ok(SuccessResponse.Create<object>(
                data: null,
                message: "User status updated successfully",
                statusCode: 200));
        }
    }

}
