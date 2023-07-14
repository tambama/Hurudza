using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Hurudza.Common.Sms.Clickatell;

public class Api
{
    //This function is in charge of converting the data into a json array and sending it to the rest sending controller.
    public static string SendSms(string token, Dictionary<string, string> @params)
    {
        @params["to"] = CreateRecipientList(@params["to"]);
        var jsonArray = JsonConvert.SerializeObject(@params, Formatting.None);
        jsonArray = jsonArray.Replace("\\\"", "\"").Replace("\"[", "[").Replace("]\"", "]");
        return Rest.Post(token, jsonArray);
    }

    //This function converts the recipients list into an array string so it can be parsed correctly by the json array.
    private static string CreateRecipientList(string to)
    {
        var tmp = to.Split(',');
        to = "[\"";
        to = to + string.Join("\",\"", tmp);
        to = to + "\"]";
        return to;
    }

}