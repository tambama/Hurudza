using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Hurudza.Common.Services.Interfaces;

namespace Common.Services.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirst(c => c.Type == ClaimTypes.PrimarySid)?.Value ?? string.Empty;
        }
        public string UserId { get; }
    }
}
