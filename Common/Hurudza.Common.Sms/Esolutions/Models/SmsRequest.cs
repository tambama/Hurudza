namespace Hurudza.Common.Sms.Esolutions.Models;

public class SmsRequest
{
    public required string Originator { get; set; }
    public required string Destination { get; set; }
    public required string MessageText { get; set; }
    public required string MessageReference { get; set; }
    public required string MessageDate { get; set; }
    public string? MessageValidity { get; set; }
    public string? SendDateTime { get; set; }
}