using System.Net;
using System.Text.Json;
using Hurudza.Data.Models.ViewModels.UserManagement;
using Hurudza.UI.Web.Api.Interfaces;

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
            await _apiCall.Add<UserViewModel, LoginViewModel>(await _apiCall.GetHttpClient(), "authentication/login",
                login);

        return response.Status == (int)HttpStatusCode.OK ? "Success" : "Failed";
    }
}