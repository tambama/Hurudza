namespace Hurudza.Common.Emails.Models;

public class EmailResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}