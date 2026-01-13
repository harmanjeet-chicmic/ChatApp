using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Auth
{
    /// <summary>
    /// Validates Google ID tokens and extracts user info.
    /// </summary>
    public class GoogleTokenValidator
    {
        private readonly IConfiguration _config;

        public GoogleTokenValidator(IConfiguration config)
        {
            _config = config;
        }

        public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _config["GoogleAuth:ClientId"] }
            };

            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
    }
}
