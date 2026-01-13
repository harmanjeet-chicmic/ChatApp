namespace ChatApp.Application.DTOs.Auth
{
    /// <summary>
    /// Represents a forgot password OTP request.
    /// </summary>
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = null!;
    }
}
