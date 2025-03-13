namespace Hurudza.Common.Sms.Esolutions.Models;

public class SmsResponse
{
    public required string Originator { get; set; }
    public required string Destination { get; set; }
    public required string MessageText { get; set; }
    public required string MessageId { get; set; }
    public required string MessageReference { get; set; }
    public required string Status { get; set; }
    public required string MessageDate { get; set; }
}