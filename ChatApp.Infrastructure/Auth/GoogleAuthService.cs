using Google.Apis.Auth;

public class GoogleAuthService : IGoogleAuthService
{
    public async Task<(string Email, string Name)> ValidateAsync(string idToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

        return (payload.Email, payload.Name);
    }
}
