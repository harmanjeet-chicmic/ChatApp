using ChatApp.API.Extensions;
using ChatApp.Application.Common;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatApp.API.Controllers
{
    /// <summary>
    /// Exposes authentication and account-related API endpoints.
    /// Handles user registration, login, social login,
    /// password recovery, and account security actions.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes the authentication controller with required services.
        /// </summary>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ============================================================
        // REGISTRATION & LOGIN
        // ============================================================

        /// <summary>
        /// Registers a new user using email and password.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            await _authService.RegisterAsync(
                request.FullName,
                request.Email,
                request.Password);

            return Ok(SuccessResponse.Create<object>(
                data: null,
                message: "User registered successfully",
                statusCode: 201));
        }

        /// <summary>
        /// Logs a user in using email and password.
        /// Returns a JWT token on successful authentication.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var token = await _authService.LoginAsync(
                request.Email,
                request.Password);

            return Ok(SuccessResponse.Create(
                data: new { token },
                message: "Login successful",
                statusCode: 200));
        }

        /// <summary>
        /// Logs a user in using a Google ID token.
        /// If the user does not exist, a new account is created automatically.
        /// </summary>
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginRequestDto request)
        {
            var token = await _authService.LoginWithGoogleAsync(request.IdToken);

            return Ok(SuccessResponse.Create(
                data: new { token },
                message: "Google login successful",
                statusCode: 200));
        }

        // ============================================================
        // PASSWORD RECOVERY
        // ============================================================

        /// <summary>
        /// Sends a one-time password (OTP) to the user's registered email
        /// to allow secure password reset.
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto request)
        {
            await _authService.SendForgotPasswordOtpAsync(request.Email);

            return Ok(SuccessResponse.Create<object>(
                data: null,
                message: "OTP sent to registered email",
                statusCode: 200));
        }

        /// <summary>
        /// Resets a user's password using a valid OTP.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
        {
            await _authService.ResetPasswordAsync(
                request.Email,
                request.Otp,
                request.NewPassword);

            return Ok(SuccessResponse.Create<object>(
                data: null,
                message: "Password reset successfully",
                statusCode: 200));
        }

        // ============================================================
        // ACCOUNT SECURITY
        // ============================================================

        /// <summary>
        /// Changes the password of the currently authenticated user.
        /// The old password must be provided for verification.
        /// </summary>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            var userId = User.GetUserId().ToString();

            await _authService.ChangePasswordAsync(
                userId,
                request.OldPassword,
                request.NewPassword);

            return Ok(SuccessResponse.Create<object>(
                data: null,
                message: "Password changed successfully",
                statusCode: 200));
        }
    }
}
