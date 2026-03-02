namespace MyAcademy_JWT.Services.AuthServices
{
    public interface IAuthService
    {
        Task<(bool ok, string? error)> RegisterAsync(string email, string password, string displayName, int packageId);
        Task<(bool ok, string? token, DateTime? expiresUtc)> LoginAsync(string email, string password);
    }
}
