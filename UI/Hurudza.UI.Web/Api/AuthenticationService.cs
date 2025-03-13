using System.Net;
using Hurudza.Data.UI.Models.ViewModels.UserManagement;
using Hurudza.UI.Shared.Api.Interfaces;
using Hurudza.UI.Web.Api.Interfaces;
using Hurudza.UI.Web.Models;

namespace Hurudza.UI.Web.Api;

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