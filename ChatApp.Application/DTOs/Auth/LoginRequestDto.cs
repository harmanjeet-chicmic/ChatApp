namespace ChatApp.Application.DTOs.Auth
{
    /// <summary>
    /// Represents a user login request.
    /// </summary>
    public class LoginRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
