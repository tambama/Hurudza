namespace Hurudza.Data.UI.Models.Models;

public class EmailSettings
{
    public const string Settings = "EmailSettings";
    public string? SendgridApiKey { get; set; }
    public string? SupportEmail { get; set; }
    public int MailServerPort { get; set; }
    public bool EnableSsl { get; set; }
    public string? MailServer { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public bool UseCredentials { get; set; }
    public string? ToAddress { get; set; }
    public string? From { get; set; }
    public string? Url { get; set; }
}