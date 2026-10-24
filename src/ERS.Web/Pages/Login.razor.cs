using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization; // <- Добавить
using ERS.Localization;
using ERS.Web.Services;

namespace ERS.Web.Pages;

public partial class Login
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IAuthService Auth { get; set; } = default!;
    [Inject] private Localizer L { get; set; } = default!;
    
    // Добавить эту строку:
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private string email = "";
    private string password = "";
    private string? error;
    private bool isLoading;
    private bool showPassword;

    private async Task HandleLogin()
    {
        error = null;
        isLoading = true;

        try
        {
            await Task.Delay(800);
            
            var success = await Auth.LoginAsync(email, password);
            if (success)
            {
                if (AuthStateProvider is CustomAuthStateProvider customProvider)
                {
                    customProvider.NotifyAuthenticationStateChanged();
                }

                Nav.NavigateTo("/");
            }
            else
            {
                error = L["login.invalid"];
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }
}