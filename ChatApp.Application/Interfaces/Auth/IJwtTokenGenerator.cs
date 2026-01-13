using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Auth
{
    /// <summary>
    /// Defines JWT token generation contract.
    /// </summary>
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
