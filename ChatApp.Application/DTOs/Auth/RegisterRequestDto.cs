

namespace ChatApp.Application.DTOs.Auth
{
    /// <summary>
    /// Represents a user registration request.
    /// </summary>
    public class RegisterRequestDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
