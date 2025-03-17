using System.IdentityModel.Tokens.Jwt;
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
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, userProfile.Email),
            new Claim(ClaimTypes.Name, $"{userProfile.Firstname} {userProfile.Surname}"),
            new Claim("UserId", userProfile.Id)
        };

        // Extract role directly from the token
        if (!string.IsNullOrEmpty(userProfile.Token))
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(userProfile.Token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(userProfile.Token);
                
                // Add role claim from token
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (roleClaim != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
                    Console.WriteLine($"Adding role from token: {roleClaim.Value}");
                }
                else if (!string.IsNullOrEmpty(userProfile.Role))
                {
                    // Fallback to user role from profile
                    claims.Add(new Claim(ClaimTypes.Role, userProfile.Role));
                    Console.WriteLine($"Adding role from profile: {userProfile.Role}");
                }

                // Add FarmId claim if present
                var farmIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "FarmId");
                if (farmIdClaim != null)
                {
                    claims.Add(new Claim("FarmId", farmIdClaim.Value));
                }

                // Add all permission claims
                foreach (var permissionClaim in jwtToken.Claims.Where(c => c.Type == "Permission"))
                {
                    claims.Add(new Claim("Permission", permissionClaim.Value));
                    Console.WriteLine($"Adding permission: {permissionClaim.Value}");
                }
            }
            else
            {
                // Fallback for invalid token
                if (!string.IsNullOrEmpty(userProfile.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, userProfile.Role));
                }
            }
        }
        else
        {
            // Fallback if no token
            if (!string.IsNullOrEmpty(userProfile.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, userProfile.Role));
            }
        }

        // Add active profile farm ID if available
        var activeProfile = userProfile.Profiles?.FirstOrDefault(p => p.LoggedIn);
        if (activeProfile != null && !string.IsNullOrEmpty(activeProfile.FarmId))
        {
            if (!claims.Any(c => c.Type == "FarmId"))
            {
                claims.Add(new Claim("FarmId", activeProfile.FarmId));
            }
        }

        // If permissions weren't added from token, add them from the user profile
        if (!claims.Any(c => c.Type == "Permission") && userProfile.Permissions != null)
        {
            foreach (var permission in userProfile.Permissions)
            {
                claims.Add(new Claim("Permission", permission.ClaimValue));
                Console.WriteLine($"Adding permission from profile: {permission.ClaimValue}");
            }
        }
 
        var identity = new ClaimsIdentity(claims, "AuthCookie");
        _claimsPrincipal = new ClaimsPrincipal(identity);
        
        // Debug output
        Console.WriteLine($"Authentication state updated with {claims.Count} claims");
        Console.WriteLine($"Roles: {string.Join(", ", _claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}");
        
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    
    public void ClearAuthInfo()
    {
        _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    // Helper methods to check permissions from the current user
    public bool UserHasPermission(string permissionName)
    {
        return _claimsPrincipal.HasClaim(c => c.Type == "Permission" && c.Value == permissionName);
    }
    
    public bool UserHasAnyPermission(params string[] permissionNames)
    {
        return _claimsPrincipal.Claims
            .Where(c => c.Type == "Permission")
            .Any(c => permissionNames.Contains(c.Value));
    }
    
    public bool UserHasAllPermissions(params string[] permissionNames)
    {
        var userPermissions = _claimsPrincipal.Claims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();
        
        return permissionNames.All(p => userPermissions.Contains(p));
    }
    
    public string GetCurrentFarmId()
    {
        return _claimsPrincipal.FindFirstValue("FarmId") ?? string.Empty;
    }
    
    public string GetUserRole()
    {
        return _claimsPrincipal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
    
    public string GetUserId()
    {
        return _claimsPrincipal.FindFirstValue("UserId") ?? string.Empty;
    }
}