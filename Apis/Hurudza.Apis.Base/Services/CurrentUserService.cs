using System.Security.Claims;
using Hurudza.Common.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Hurudza.Apis.Base.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.PrimarySid) ?? string.Empty;
    }
    public string UserId { get; }
}