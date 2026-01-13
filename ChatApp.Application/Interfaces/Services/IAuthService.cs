using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces.Services
{
    /// <summary>
    /// Defines authentication and account-related operations.
    /// </summary>
    public interface IAuthService
    {
        Task RegisterAsync(string fullName, string email, string password);
        Task<string> LoginAsync(string email, string password);
        Task<string> LoginWithGoogleAsync(string googleToken);

        Task SendForgotPasswordOtpAsync(string email);
        Task ResetPasswordAsync(string email, string otp, string newPassword);
        Task ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    }
}
