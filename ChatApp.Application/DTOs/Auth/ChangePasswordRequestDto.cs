namespace ChatApp.Application.DTOs.Auth
{
    /// <summary>
    /// Represents a password change request for authenticated users.
    /// </summary>
    public class ChangePasswordRequestDto
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
