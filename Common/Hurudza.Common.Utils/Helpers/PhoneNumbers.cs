using System.Text.RegularExpressions;

namespace Hurudza.Common.Utils.Helpers;

public static class PhoneNumbers
{
    public static string GetMobilePaymentMethod(string phone)
    {
        var cleanPhone = phone.Trim().Replace(" ", string.Empty);

        return true switch
        {
            bool _ when Regex.IsMatch(cleanPhone, @"^((00|\+)?(263))?0?7(7|8)[0-9]{7}$") => "ecocash",
            bool _ when Regex.IsMatch(cleanPhone, @"^((00|\+)?(263))?0?71[0-9]{7}$") => "onemoney",
            bool _ when Regex.IsMatch(cleanPhone, @"^((00|\+)?(263))?0?73[0-9]{7}$") => "telecash",
            _ => string.Empty,
        };
    }
}
