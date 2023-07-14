using Hurudza.Common.Sms.Clickatell;
using Serilog;

namespace Hurudza.Common.Sms.Services;

public class ClickatellSmsService: ISmsService
{
    public async Task<string> Send(string to, string message)
    {
        try
        {
            //creating a dictionary to store all the parameters that needs to be sent
            var @params = new Dictionary<string, string>
            {
                //adding the parameters to the dictionary
                {"content", message},
                {"to", to}
            };

            return Api.SendSms("UN-hoFrxSQ-edCsCZ3zwNw==", @params);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return ex.Message;
        }
    }
}