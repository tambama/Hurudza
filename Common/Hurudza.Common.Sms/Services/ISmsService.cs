namespace Hurudza.Common.Sms.Services;

public interface ISmsService
{
    Task<string> Send(string to, string message);
}