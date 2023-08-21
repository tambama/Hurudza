using System.Net;
using Hurudza.Data.Models.ViewModels.UserManagement;
using Hurudza.UI.Shared.Api.Interfaces;
using Hurudza.UI.Shared.Models;
using Hurudza.UI.Web.Api.Interfaces;

namespace Hurudza.UI.Shared.Api;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApiCall _apiCall;

    public AuthenticationService(IApiCall apiCall)
    {
        _apiCall = apiCall;
    }
    
    public async Task<string> LoginAsync(LoginViewModel login)
    {
        var response =
            await _apiCall.Add<ApiResponse<UserViewModel>, LoginViewModel>(await _apiCall.GetHttpClient(), "authentication/login",
                login);

        return response.Status == (int)HttpStatusCode.OK ? "Success" : "Failed";
    }
}