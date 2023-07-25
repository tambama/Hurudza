namespace Hurudza.Data.Models.Models;

public class ApiClaim
{
    public string? Id { get; set; }
    public required string Type { get; set; }
    public required string Value { get; set; }
}