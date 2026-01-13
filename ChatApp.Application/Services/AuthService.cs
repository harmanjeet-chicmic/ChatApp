using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Application.Interfaces.Auth;
using Google.Apis.Auth;
using System;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    /// <summary>
    /// Handles authentication, registration, social login,
    /// password recovery, and security-related user actions.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
        }

        // ============================================================
        // ACCOUNT REGISTRATION
        // ============================================================

        public async Task RegisterAsync(string fullName, string email, string password)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                PasswordHash = HashPassword(password),
                IsGoogleAccount = false,
                Status = UserStatus.Offline,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }

        // ============================================================
        // LOGIN (EMAIL + PASSWORD)
        // ============================================================

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return _jwtTokenGenerator.GenerateToken(user);
        }

        // ============================================================
        // GOOGLE LOGIN
        // ============================================================

        public async Task<string> LoginWithGoogleAsync(string idToken)
        {
            // 1️⃣ Validate Google token
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            if (payload == null || string.IsNullOrEmpty(payload.Email))
                throw new UnauthorizedAccessException("Invalid Google token.");

            // 2️⃣ Find user by email
            var user = await _userRepository.GetByEmailAsync(payload.Email);

            // 3️⃣ Create account if first-time Google login
            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = payload.Name ?? payload.Email,
                    Email = payload.Email,
                    PasswordHash = string.Empty,
                    IsGoogleAccount = true,
                    Status = UserStatus.Offline,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
            }

            // 4️⃣ Generate JWT
            return _jwtTokenGenerator.GenerateToken(user);
        }

        // ============================================================
        // PASSWORD RESET (OTP FLOW)
        // ============================================================

        public async Task SendForgotPasswordOtpAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email)
                ?? throw new Exception("User not found");

            var otp = new Random().Next(100000, 999999).ToString();

            user.PasswordResetOtp = otp;
            user.PasswordResetOtpExpiry = DateTime.UtcNow.AddMinutes(10);

            await _userRepository.UpdateAsync(user);

            Console.WriteLine($"Password reset OTP for {email}: {otp}");

            await _emailService.SendEmailAsync(
                email,
                "ChatApp Password Reset OTP",
                $"<h3>Your OTP is: {otp}</h3><p>Valid for 10 minutes</p>"
            );
        }

        public async Task ResetPasswordAsync(string email, string otp, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email)
                ?? throw new Exception("User not found");

            if (user.PasswordResetOtp == null || user.PasswordResetOtpExpiry == null)
                throw new Exception("No password reset request found");

            if (user.PasswordResetOtp != otp)
                throw new Exception("Invalid OTP");

            if (user.PasswordResetOtpExpiry < DateTime.UtcNow)
                throw new Exception("OTP has expired");

            user.PasswordHash = HashPassword(newPassword);
            user.PasswordResetOtp = null;
            user.PasswordResetOtpExpiry = null;

            await _userRepository.UpdateAsync(user);
        }

        // ============================================================
        // PASSWORD CHANGE (LOGGED-IN USER)
        // ============================================================

        public async Task ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var id = Guid.Parse(userId);

            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new UnauthorizedAccessException("User not found.");

            if (!VerifyPassword(oldPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Old password is incorrect.");

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        // ============================================================
        // PRIVATE HELPERS
        // ============================================================

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
