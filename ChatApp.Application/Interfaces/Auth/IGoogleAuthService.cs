public interface IGoogleAuthService
{
    Task<(string Email, string Name)> ValidateAsync(string idToken);
}
