namespace ChatApp.Application.DTOs.Auth
{
    /// <summary>
    /// Represents a Google authentication request.
    /// </summary>
    public class GoogleLoginRequestDto
    {
        public string IdToken { get; set; } = null!;
    }
}
