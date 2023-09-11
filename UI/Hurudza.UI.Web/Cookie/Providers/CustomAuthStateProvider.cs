using System.Security.Claims;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Components.Authorization;

namespace Hurudza.UI.Web.Cookie.Providers;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return await Task.FromResult(new AuthenticationState(_claimsPrincipal));
    }
    
    public void SetAuthInfo(UserViewModel userProfile)
    {
        var identity = new ClaimsIdentity(new[]{
            new Claim(ClaimTypes.Email, userProfile.Email),
            new Claim(ClaimTypes.Name, $"{userProfile.Firstname} {userProfile.Surname}"),
            new Claim("UserId", userProfile.Id)
        }, "AuthCookie");
 
        _claimsPrincipal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    
    public void ClearAuthInfo()
    {
        _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}