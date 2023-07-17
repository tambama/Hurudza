namespace Hurudza.Data.Context.Data;

public enum ApiClaimTypes
{
    Farm,
    User,
    Field
}

public static class ApiClaims
{
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
