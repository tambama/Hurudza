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
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = FarmCreate},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = FarmRead},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = FarmUpdate},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = FarmDelete},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = FarmView},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = FarmViewAll},
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = FarmManage},
            
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = UserCreate},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = UserRead},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = UserUpdate},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = UserDelete},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = UserView},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = UserViewAll},
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = UserManage},
            
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = FieldCreate},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = FieldRead},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = FieldUpdate},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = FieldDelete},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = FieldView},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = FieldViewAll},
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = FieldManage},
        };
    }
    
    // Farm claims
    public const string FarmCreate = "Farm.Create";
    public const string FarmRead = "Farm.Read";
    public const string FarmUpdate = "Farm.Update";
    public const string FarmDelete = "Farm.Delete";
    public const string FarmView = "Farm.View";
    public const string FarmViewAll = "Farm.ViewAll";
    public const string FarmManage = "Farm.Manage";

    // User claims
    public const string UserCreate = "User.Create";
    public const string UserRead = "User.Read";
    public const string UserUpdate = "User.Update";
    public const string UserDelete = "User.Delete";
    public const string UserView = "User.View";
    public const string UserViewAll = "User.ViewAll";
    public const string UserManage = "User.Manage";
    
    // Field claims
    public const string FieldCreate = "Field.Create";
    public const string FieldRead = "Field.Read";
    public const string FieldUpdate = "Field.Update";
    public const string FieldDelete = "Field.Delete";
    public const string FieldView = "Field.View";
    public const string FieldViewAll = "Field.ViewAll";
    public const string FieldManage = "Field.Manage";
}
