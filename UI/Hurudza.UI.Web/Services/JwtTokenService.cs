using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Hurudza.UI.Web.Services;

public class JwtTokenService
{
    public IEnumerable<Claim> ParseJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return Enumerable.Empty<Claim>();

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims;
        }
        catch
        {
            return Enumerable.Empty<Claim>();
        }
    }

    public string ExtractUserIdFromToken(string token)
    {
        var claims = ParseJwtToken(token);
        return claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value ?? string.Empty;
    }

    public string ExtractRoleFromToken(string token)
    {
        var claims = ParseJwtToken(token);
        return claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
    }

    public string ExtractFarmIdFromToken(string token)
    {
        var claims = ParseJwtToken(token);
        return claims.FirstOrDefault(c => c.Type == "FarmId")?.Value ?? string.Empty;
    }

    public List<string> ExtractPermissionsFromToken(string token)
    {
        var claims = ParseJwtToken(token);
        return claims.Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToList();
    }

    public bool TokenHasPermission(string token, string permission)
    {
        var permissions = ExtractPermissionsFromToken(token);
        return permissions.Contains(permission);
    }

    public bool TokenHasAnyPermission(string token, params string[] permissions)
    {
        var userPermissions = ExtractPermissionsFromToken(token);
        return permissions.Any(p => userPermissions.Contains(p));
    }

    public bool TokenHasAllPermissions(string token, params string[] permissions)
    {
        var userPermissions = ExtractPermissionsFromToken(token);
        return permissions.All(p => userPermissions.Contains(p));
    }

    public DateTime GetTokenExpirationDate(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    public bool IsTokenExpired(string token)
    {
        var expirationDate = GetTokenExpirationDate(token);
        return expirationDate == DateTime.MinValue || expirationDate < DateTime.UtcNow;
    }
}