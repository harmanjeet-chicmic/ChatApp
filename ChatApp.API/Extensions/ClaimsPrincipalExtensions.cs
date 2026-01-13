using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ChatApp.API.Extensions
{
    /// <summary>
    /// Provides helper methods for accessing user claims.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId =
                user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.Parse(userId!);
        } 
    }
}
