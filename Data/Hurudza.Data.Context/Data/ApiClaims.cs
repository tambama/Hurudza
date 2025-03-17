using Hurudza.Data.Data;
using Hurudza.Data.Models.Models;

namespace Hurudza.Data.Context.Data;

public enum ApiClaimTypes
{
    Farm,
    User,
    Field
}

public static class ApiClaims
{
    public static List<IdentityClaim> GetClaims()
    {
        return new List<IdentityClaim>()
        {
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmCreate},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmRead},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmUpdate},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmDelete},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmView},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmViewAll},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmManage},
            
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserCreate},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserRead},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserUpdate},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserDelete},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserView},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserViewAll},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserManage},
            
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldCreate},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldRead},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldUpdate},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldDelete},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldView},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldViewAll},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldManage},
        };
    }
}
