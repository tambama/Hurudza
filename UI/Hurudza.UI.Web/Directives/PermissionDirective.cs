using Hurudza.UI.Web.Cookie.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;

namespace Hurudza.UI.Web.Directives;

[Authorize]
public class PermissionDirective : ComponentBase
{
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; }
    
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public string Permission { get; set; }
    [Parameter] public string[] AnyPermission { get; set; }
    [Parameter] public string[] AllPermissions { get; set; }
    [Parameter] public RenderFragment ElseContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var authProvider = AuthStateProvider as CustomAuthStateProvider;
        bool hasPermission = false;
        
        if (authProvider != null)
        {
            if (!string.IsNullOrEmpty(Permission))
            {
                hasPermission = authProvider.UserHasPermission(Permission);
            }
            else if (AnyPermission != null && AnyPermission.Length > 0)
            {
                hasPermission = authProvider.UserHasAnyPermission(AnyPermission);
            }
            else if (AllPermissions != null && AllPermissions.Length > 0)
            {
                hasPermission = authProvider.UserHasAllPermissions(AllPermissions);
            }
        }

        if (hasPermission)
        {
            builder.AddContent(0, ChildContent);
        }
        else if (ElseContent != null)
        {
            builder.AddContent(0, ElseContent);
        }
    }
}