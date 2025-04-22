using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Hurudza.UI.Web.Api.Interfaces;
using Hurudza.UI.Web.Models;

namespace Hurudza.UI.Web.Cookie.Providers;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly IApiCall _apiCall;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    private ClaimsPrincipal _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
    
    public CustomAuthStateProvider(ILocalStorageService localStorage, IApiCall apiCall)
    {
        _localStorage = localStorage;
        _apiCall = apiCall;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("token");
            
            // If no token or token is empty, return anonymous user
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("No token found, returning anonymous user");
                return new AuthenticationState(_anonymous);
            }
            
            // Try to create claims from the saved token
            var identity = CreateClaimsIdentityFromToken(token);
            
            // If we got a valid identity from the token, use it
            if (identity != null && identity.IsAuthenticated)
            {
                _claimsPrincipal = new ClaimsPrincipal(identity);
                Console.WriteLine($"Token is valid, user authenticated as: {identity.Name}");
                
                // Log all claims for debugging
                foreach (var claim in identity.Claims)
                {
                    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("Token invalid or expired, returning anonymous user");
                // Invalid token - return anonymous
                return new AuthenticationState(_anonymous);
            }
            
            return new AuthenticationState(_claimsPrincipal);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAuthenticationStateAsync: {ex.Message}");
            return new AuthenticationState(_anonymous);
        }
    }
    
    public void SetAuthInfo(UserViewModel userProfile)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id),
                new Claim(ClaimTypes.Email, userProfile.Email),
                new Claim(ClaimTypes.Name, userProfile.Fullname),
                new Claim("UserId", userProfile.Id)
            };

            // Extract claims from token if present
            if (!string.IsNullOrEmpty(userProfile.Token))
            {
                ExtractClaimsFromToken(userProfile.Token, claims);
            }
            
            // Add Role directly from profile if available
            if (!string.IsNullOrEmpty(userProfile.Role))
            {
                if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == userProfile.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, userProfile.Role));
                    Console.WriteLine($"Added role from profile: {userProfile.Role}");
                }
            }
            
            // Add FarmId from active profile if available
            var activeProfile = userProfile.Profiles?.FirstOrDefault(p => p.LoggedIn);
            if (activeProfile != null && !string.IsNullOrEmpty(activeProfile.FarmId))
            {
                claims.Add(new Claim("FarmId", activeProfile.FarmId));
                claims.Add(new Claim("FarmName", activeProfile.Farm));
            }
            
            // Add permissions from user profile if not already added from token
            if (userProfile.Permissions != null && userProfile.Permissions.Any())
            {
                foreach (var permission in userProfile.Permissions)
                {
                    claims.Add(new Claim("Permission", permission.ClaimValue));
                    Console.WriteLine($"Adding permission from profile: {permission.ClaimValue}");
                }
            }
     
            var identity = new ClaimsIdentity(claims, "AuthCookie");
            _claimsPrincipal = new ClaimsPrincipal(identity);
            
            Console.WriteLine($"Authentication state updated for user: {userProfile.Fullname}");
            Console.WriteLine($"Claims count: {claims.Count}");
            Console.WriteLine($"Roles: {string.Join(", ", _claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}");
            
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SetAuthInfo: {ex.Message}");
        }
    }
    
    public void ClearAuthInfo()
    {
        _claimsPrincipal = _anonymous;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        Console.WriteLine("Authentication state cleared");
    }
    
    public async Task RefreshAuthState()
    {
        try
        {
            // Get current user ID from claims
            var userId = _claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                        _claimsPrincipal.FindFirstValue("UserId");
            
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("Cannot refresh auth state: No user ID found in claims");
                return;
            }
            
            // Call API to get current profile
            var client = await _apiCall.GetHttpClient();
            var response = await _apiCall.Get<ApiResponse<UserViewModel>>(client, $"authentication/getloggedinprofile");
            
            if (response?.Result != null)
            {
                // Update auth state with refreshed profile
                SetAuthInfo(response.Result);
                Console.WriteLine("Auth state refreshed successfully");
            }
            else
            {
                Console.WriteLine("Failed to refresh auth state: API response was null or invalid");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refreshing auth state: {ex.Message}");
        }
    }
    
    // Helper to create claims identity from token
    private ClaimsIdentity CreateClaimsIdentityFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token))
            {
                Console.WriteLine("Invalid token format");
                return null;
            }
            
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            // Check if token is expired
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                Console.WriteLine("Token has expired");
                return null;
            }
            
            // Extract claims from token
            var claims = jwtToken.Claims.ToList();
            
            // Add standard claims if missing
            if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            {
                var nameId = claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.PrimarySid);
                if (nameId != null)
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, nameId.Value));
                }
            }
            
            if (claims.Any())
            {
                return new ClaimsIdentity(claims, "jwt");
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing token: {ex.Message}");
            return null;
        }
    }
    
    // Helper to extract claims from token
    private void ExtractClaimsFromToken(string token, List<Claim> claims)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                
                // Process role claims
                var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role);
                foreach (var roleClaim in roleClaims)
                {
                    if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == roleClaim.Value))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
                        Console.WriteLine($"Added role from token: {roleClaim.Value}");
                    }
                }
                
                // Process FarmId claim
                var farmIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "FarmId");
                if (farmIdClaim != null && !claims.Any(c => c.Type == "FarmId"))
                {
                    claims.Add(new Claim("FarmId", farmIdClaim.Value));
                    Console.WriteLine($"Added FarmId from token: {farmIdClaim.Value}");
                }
                
                // Process permission claims
                foreach (var permissionClaim in jwtToken.Claims.Where(c => c.Type == "Permission"))
                {
                    if (!claims.Any(c => c.Type == "Permission" && c.Value == permissionClaim.Value))
                    {
                        claims.Add(new Claim("Permission", permissionClaim.Value));
                        Console.WriteLine($"Added permission from token: {permissionClaim.Value}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Token could not be read as JWT");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting claims from token: {ex.Message}");
        }
    }

    // Helper methods to check permissions
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
    
    public string GetCurrentFarmName()
    {
        return _claimsPrincipal.FindFirstValue("FarmName") ?? string.Empty;
    }
    
    public string GetUserRole()
    {
        return _claimsPrincipal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
    
    public string GetUserId()
    {
        return _claimsPrincipal.FindFirstValue("UserId") ?? 
               _claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
               _claimsPrincipal.FindFirstValue(ClaimTypes.PrimarySid) ?? string.Empty;
    }
}