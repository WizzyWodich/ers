namespace ERS.Web.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> RegisterAsync(string firstName, string lastName, string email, string password, string groupId);
}