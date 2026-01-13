namespace ChatApp.Application.DTOs.Auth
{
    /// <summary>
    /// Represents a password reset request using OTP.
    /// </summary>
    public class ResetPasswordRequestDto
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
