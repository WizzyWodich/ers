namespace ERS.Web.Services;

public class AuthService : IAuthService
{
    public async Task<bool> LoginAsync(string email, string password)
    {
        await Task.Delay(800);
        
        return email == "admin@college.ua" && password == "admin";
    }

    public Task LogoutAsync() => Task.CompletedTask;

    public Task<string?> GetTokenAsync() => Task.FromResult<string?>("fake-jwt-token");

    public Task<bool> IsAuthenticatedAsync() => Task.FromResult(true);

    public async Task<bool> RegisterAsync(string firstName, string lastName, string email, string password, string groupId)
{
    await Task.Delay(1000); 
    return true; 
}
}