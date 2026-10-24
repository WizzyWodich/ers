using Microsoft.JSInterop;

namespace ERS.Web.Services;

public class AuthService : IAuthService
{
    private readonly IJSRuntime _js;
    private const string TokenKey = "ers_jwt";

    public AuthService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        await Task.Delay(800);
        
        if (email == "admin@college.ua" && password == "admin")
        {
            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, "mock-jwt-token-12345");
            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public Task<bool> RegisterAsync(string firstName, string lastName, string email, string password, string groupId)
    {
        throw new NotImplementedException();
    }
}