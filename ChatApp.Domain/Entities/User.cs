using System.Dynamic;
using ChatApp.Domain.Common;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities;

public class User : AuditableEntity
{
    public String FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public UserStatus Status { get; set; }
    public Boolean IsGoogleAccount { get; set; }

    public string? PasswordResetOtp { get; set; }
    public DateTime? PasswordResetOtpExpiry { get; set; }

}